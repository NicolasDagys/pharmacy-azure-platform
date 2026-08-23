
---------- TABLES ----------

CREATE TABLE Category
(
    CodeCat VARCHAR(6) NOT NULL,
    NameCat VARCHAR(50) NOT NULL,
    Active BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT PK_Category PRIMARY KEY (CodeCat),
    CONSTRAINT CHK_Category_CodeCat CHECK (CodeCat LIKE '[A-Z][A-Z][A-Z][0-9][0-9][0-9]'),
    CONSTRAINT CHK_Category_NameCat CHECK (LEN(NameCat) > 2)
);
GO

CREATE TABLE [Product]
(
    CodProd VARCHAR(10) NOT NULL,
    NameProd VARCHAR(25) NOT NULL,
    PriceProd DECIMAL(10, 2) NOT NULL,
    ExpDateProd DATE NOT NULL,
    PresentationTypeProd VARCHAR(10) NOT NULL,
    SizeProd INT NOT NULL,
    CodeCat VARCHAR(6) NOT NULL,
	StockQty INT NOT NULL DEFAULT 0,
    Active BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT PK_Product PRIMARY KEY (CodProd),
    CONSTRAINT CHK_Product_CodProd CHECK (CodProd LIKE '[A-Z][A-Z][A-Z][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
    CONSTRAINT CHK_Product_NameProd CHECK (LEN(NameProd) > 2),
    CONSTRAINT CHK_Product_PriceProd CHECK (PriceProd > 0),
    --CONSTRAINT CHK_Product_ExpDateProd CHECK (ExpDateProd > GETDATE()), --this validation must be done in the api & front end
    CONSTRAINT CHK_Product_PresentationTypeProd CHECK (PresentationTypeProd IN ('Tablets', 'Syrups', 'Creams', 'Inhalers')),
    CONSTRAINT CHK_Product_SizeProd CHECK (SizeProd > 0),
	CONSTRAINT CHK_Product_StockQty CHECK (StockQty >= 0),
    CONSTRAINT FK_Product_Category FOREIGN KEY (CodeCat) REFERENCES Category(CodeCat)
);
GO

CREATE TABLE Customer
(
    IdCus VARCHAR(9) NOT NULL,
    NameCus VARCHAR(20) NOT NULL,
    PaymentTokenCus VARCHAR(255) NULL,        -- Token encriptado, nunca número real de tarjeta
    MailCus VARCHAR(100) NOT NULL,
    PhoneCus VARCHAR(20) NULL,
    
    CONSTRAINT PK_Customer PRIMARY KEY (IdCus),
    CONSTRAINT CHK_Customer_IdCus CHECK (IdCus LIKE '[1-6][0-9][0-9][0-9][0-9][0-9][0-9]-[0-9]' 
                                   OR IdCus LIKE '[1-9][0-9][0-9][0-9][0-9][0-9]-[0-9]'),
    CONSTRAINT CHK_Customer_NameCus CHECK (LEN(NameCus) > 2),
    CONSTRAINT CHK_Customer_MailCus CHECK (MailCus LIKE '%_@__%.__%'), -- Basic validation, More in-depth validations should be done in the API
    CONSTRAINT CHK_Customer_PhoneCus CHECK (PhoneCus IS NULL OR PhoneCus NOT LIKE '%[^0-9]%')
);
GO

CREATE TABLE Invoice
(
    NumbInv INT IDENTITY(1,1) NOT NULL,
    DateInv DATE NOT NULL DEFAULT GETDATE(),
    ShipmentAddressInv VARCHAR(50) NOT NULL,
    TotalInv DECIMAL(10, 2) NOT NULL DEFAULT 0,
    IdCus VARCHAR(9) NOT NULL,
    
    CONSTRAINT PK_Invoice PRIMARY KEY (NumbInv),
    CONSTRAINT CHK_Invoice_ShipmentAddressInv CHECK (LEN(ShipmentAddressInv) > 4),
    CONSTRAINT CHK_Invoice_TotalInv CHECK (TotalInv >= 0),
    CONSTRAINT FK_Invoice_Customer FOREIGN KEY (IdCus) REFERENCES Customer(IdCus)
);
GO

CREATE TABLE InvoiceLine
(
    NumbInv INT NOT NULL,
    CodProd VARCHAR(10) NOT NULL,
    Cant INT NOT NULL,
	UnitPrice DECIMAL(10,2) NOT NULL,
    
    CONSTRAINT PK_Line PRIMARY KEY (NumbInv, CodProd),
    CONSTRAINT CHK_Line_Cant CHECK (Cant > 0),
    CONSTRAINT FK_Line_Invoice FOREIGN KEY (NumbInv) REFERENCES Invoice(NumbInv),
    CONSTRAINT FK_Line_Product FOREIGN KEY (CodProd) REFERENCES [Product](CodProd),
	CONSTRAINT CHK_Line_UnitPrice CHECK (UnitPrice > 0)
);
GO

CREATE TABLE OrderStatus
(
    NumbSta INT NOT NULL,
    NameSta VARCHAR(50) NOT NULL,
    
    CONSTRAINT PK_OrderStatus PRIMARY KEY (NumbSta),
    CONSTRAINT CHK_OrderStatus_NameSta CHECK (LEN(NameSta) > 3)
);
GO

CREATE TABLE Assignment
(
    NumbInv INT NOT NULL,
    NumbSta INT NOT NULL,
    DateTimeStatus DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_Assignment PRIMARY KEY (NumbInv, NumbSta),
    CONSTRAINT FK_Assignment_Invoice FOREIGN KEY (NumbInv) REFERENCES Invoice(NumbInv),
    CONSTRAINT FK_Assignment_OrderStatus FOREIGN KEY (NumbSta) REFERENCES OrderStatus(NumbSta)
);
GO

CREATE TABLE Employee
(
    UserEmp VARCHAR(50) NOT NULL,
    NameEmp VARCHAR(50) NOT NULL,
    PassHashEmp VARCHAR(255) NOT NULL,        -- Hash BCrypt/Argon2, nunca texto plano
	RoleEmp VARCHAR(20) NOT NULL, 
    
    CONSTRAINT PK_Employee PRIMARY KEY (UserEmp),
    CONSTRAINT CHK_Employee_UserEmp CHECK (LEN(UserEmp) > 2),
    CONSTRAINT CHK_Employee_NameEmp CHECK (LEN(NameEmp) > 2),
    CONSTRAINT CHK_Employee_PassHashEmp CHECK (LEN(PassHashEmp) > 4),
	CONSTRAINT CHK_Employee_RoleEmp CHECK (RoleEmp IN ('Admin','Pharmacist','Sales')) --autentication will be implemented with JWT 
);
GO



----------------------------------------- SP--------------------------------------------------------

--Table Valued Parameter (tvp) pararecibir multiples lineas
CREATE TYPE InvoiceLineType AS TABLE
(
    CodProd VARCHAR(10),
    Cant INT
);
GO

--CreateInvoice (trn) VALIDACIONES PREVIAS, INSERTAR 
CREATE PROCEDURE sp_CreateInvoice
(
    @IdCus VARCHAR(9),
    @ShipmentAddressInv VARCHAR(50),
    @Lines InvoiceLineType READONLY
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NumbInv INT;
    DECLARE @Total DECIMAL(10,2);

    ----------------------------------------------------
    -- Validations
    ----------------------------------------------------

    IF NOT EXISTS
    (
        SELECT 1
        FROM Customer
        WHERE IdCus = @IdCus
    )
    BEGIN
        RETURN -1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM @Lines L
        LEFT JOIN Product P
            ON P.CodProd = L.CodProd
        WHERE P.CodProd IS NULL
    )
    BEGIN
        RETURN -2;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM @Lines L
        INNER JOIN Product P
            ON P.CodProd = L.CodProd
        WHERE P.StockQty < L.Cant
    )
    BEGIN
        RETURN -3;
    END;

    BEGIN TRY

        BEGIN TRANSACTION;

        ----------------------------------------------------
        -- Invoice
        ----------------------------------------------------

        INSERT INTO Invoice
        (
            ShipmentAddressInv,
            TotalInv,
            IdCus
        )
        VALUES
        (
            @ShipmentAddressInv,
            0,
            @IdCus
        );

        SET @NumbInv = SCOPE_IDENTITY();

        ----------------------------------------------------
        -- Lines
        ----------------------------------------------------

        INSERT INTO InvoiceLine
        (
            NumbInv,
            CodProd,
            Cant,
            UnitPrice
        )
        SELECT
            @NumbInv,
            P.CodProd,
            L.Cant,
            P.PriceProd
        FROM @Lines L
        INNER JOIN Product P
            ON P.CodProd = L.CodProd;

        ----------------------------------------------------
        -- Initial Status
        ----------------------------------------------------

        INSERT INTO Assignment
        (
            NumbInv,
            NumbSta
        )
        VALUES
        (
            @NumbInv,
            1
        );

        ----------------------------------------------------
        -- Discount Stock
        ----------------------------------------------------

        UPDATE P
        SET
            P.StockQty = P.StockQty - L.Cant
        FROM Product P
        INNER JOIN @Lines L
            ON P.CodProd = L.CodProd;

        ----------------------------------------------------
        -- Calculate Total
        ----------------------------------------------------

        SELECT
            @Total = SUM(Cant * UnitPrice)
        FROM InvoiceLine
        WHERE NumbInv = @NumbInv;

        UPDATE Invoice
        SET TotalInv = @Total
        WHERE NumbInv = @NumbInv;

        COMMIT TRANSACTION;

        ----------------------------------------------------
        -- Return Invoice Number
        ----------------------------------------------------

        SELECT @NumbInv AS InvoiceNumber;

        RETURN 0;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH

END
GO
