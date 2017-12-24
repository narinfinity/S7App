import { InjectionToken } from '@angular/core';
import { AppConfig } from './model/app.config.model';

const APP_CONFIG = new InjectionToken<AppConfig>('app.config');
const useValue = function() {    
    return {
        baseUrl: 'https://localhost:44329',
        clientId: 'angular_client_s6BhdRkqt3',
        scope: 'openid offline_access profile roles',
        filterState: { page: 1, pageSize: 5, orderBy: { prop: null, asc: null } },
        pageSizes: [5, 10, 20]
    };
};

export { AppConfig, APP_CONFIG, useValue };
