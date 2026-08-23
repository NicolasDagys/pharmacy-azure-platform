import { Component, OnInit, inject } from '@angular/core';

import { CommonModule } from '@angular/common';

import { DashboardService } from '../../services/dashboard.service';

import { Dashboard } from '../../models/dashboard';


import { StatCardComponent } from '../../../../shared/components/stat-card/stat-card';

@Component({

    selector: 'app-dashboard',

    standalone: true,

    imports:[
    CommonModule,
    StatCardComponent
    ],

    templateUrl: './dashboard.html',

    styleUrl: './dashboard.css'

})
export class DashboardComponent implements OnInit {

    private dashboardService = inject(DashboardService);

    dashboard?: Dashboard;

    loading = true;

    error = false;

    ngOnInit(): void {

        this.loadDashboard();

    }

    loadDashboard(): void {

        this.loading = true;

        this.error = false;

        this.dashboardService.getDashboard().subscribe({

            next: (response) => {

                this.dashboard = response;

                this.loading = false;

            },

            error: (err) => {

                console.error(err);

                this.error = true;

                this.loading = false;

            }

        });

    }

}