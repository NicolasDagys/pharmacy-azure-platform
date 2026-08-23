import { HttpParams } from '@angular/common/http';
import { QueryParameters } from '../models/query-parameters';

export function buildQueryParams(query: QueryParameters): HttpParams {

    let params = new HttpParams()
        .set('PageNumber', query.page)
        .set('PageSize', query.pageSize);

    if (query.search) {

        params = params.set(
            'Search',
            query.search
        );

    }

    if (query.sortBy) {

        params = params.set(
            'SortBy',
            query.sortBy
        );

    }

    params = params.set(
        'Desc',
        query.descending ?? false
    );

    return params;

}