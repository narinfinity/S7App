import 'rxjs/add/operator/map';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/first';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/filter';
import 'rxjs/add/observable/of';
import 'rxjs/add/observable/interval';
import 'rxjs/add/observable/throw';
import { Component, Inject, Injectable, OnInit } from '@angular/core';
import { Http, Headers, RequestOptions, Response, URLSearchParams } from '@angular/http';
import { AppConfig, APP_CONFIG } from '../../app/app.config';

import { Observable } from 'rxjs/Observable';

import { AuthTokenModel } from '../model/auth.tokens.model';
import { AuthService } from './auth.service';

@Injectable()
export class PlayerService implements OnInit {
        
    private config: AppConfig
    private requestOptions: RequestOptions;

    constructor(
        private http: Http,
        private authService: AuthService,
        @Inject(APP_CONFIG) config: AppConfig
    ) {
        this.config = config;
    }

    ngOnInit() {

    }
    private createRequestOptions(addHeaders: any = {}): RequestOptions {
        let tokens = { token_type: '', access_token: '' };
        this.authService.tokens$.subscribe((res: AuthTokenModel) => {
            if (!res) return;
            tokens = {
                token_type: res.token_type,
                access_token: res.access_token
            };
        });
        const headers = new Headers(Object.assign({
            'Authorization': `${tokens.token_type} ${tokens.access_token}`,
            'Accept': 'application/json; charset=utf-8',
        }, addHeaders));
        return new RequestOptions({ headers: headers });
    }
    private getParams(args: any = {}, method = 'get'): string {
        const params = new URLSearchParams();
        Object.keys(args).filter(a => !!a).forEach(key => {
            const keys = Object.keys(args[key]);
            if (keys.length > 0 && Object.prototype.toString.call(args[key]) !== '[object String]')
            {
                if (Object.prototype.toString.call(args[key]) === '[object Array]') keys.forEach(k => params.append(key, args[key][k]));
                else keys.forEach(k => params.append(`${key}.${k}`, args[key][k]));
            }            
            else params.append(key, args[key])
        });

        return params.toString().length ? (method === 'get' ? `?${params.toString()}` : params.toString()) : '';
    }

    //Get: /api/player
    getTeams(args: any): any {
        
        return this.http.get(`${this.config.baseUrl}/api/team/${this.getParams(args)}`, this.createRequestOptions());
    }
    // Get: /api/player
    getPlayers(args: any): Observable<any> { 
        return this.http.get(`${this.config.baseUrl}/api/player${this.getParams(args)}`, this.createRequestOptions());
    }

    deletePlayer(playerId: number): Observable<any> {        
        return this.http.delete(`${this.config.baseUrl}/api/player/${playerId}`, this.createRequestOptions());
    }

    addOrUpdatePlayer(args: any): Observable<any> {        
        const addHeaders = { 'content-type': 'application/x-www-form-urlencoded' };
        return this.http.post(`${this.config.baseUrl}/api/player`, this.getParams(args, 'post'), this.createRequestOptions(addHeaders));
    }
}

