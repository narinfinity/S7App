import 'rxjs/add/operator/map';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/first';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/filter';
import 'rxjs/add/observable/of';
import 'rxjs/add/observable/interval';
import 'rxjs/add/observable/throw';
import { Component, Inject, Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response, URLSearchParams } from '@angular/http';
import { AppConfig, APP_CONFIG } from '../../app/app.config';

import { Observable } from 'rxjs/Observable';
import { Subscriber } from 'rxjs/Subscriber';
import { Subscription } from 'rxjs/Subscription';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

import { RefreshGrantModel } from '../model/refresh.grant.model';
import { ProfileModel } from '../model/profile.model';
import { AuthStateModel } from '../model/auth.state.model';
import { AuthTokenModel } from '../model/auth.tokens.model';
import { RegisterModel } from '../model/register.model';
import { LoginModel } from '../model/login.model';
import { JwtHelper } from 'angular2-jwt';

@Injectable()
export class AuthService {
    private jwtHelper: JwtHelper = new JwtHelper();
    private initalState: AuthStateModel | any = { profile: null, tokens: null, authReady: false };
    private authReady$ = new BehaviorSubject<boolean>(false);
    private state: BehaviorSubject<AuthStateModel>;
    private refreshSubscription$: Subscription;
    private config: AppConfig;
    state$: Observable<AuthStateModel>;
    tokens$: Observable<AuthTokenModel | any>;
    profile$: Observable<ProfileModel | any>;
    public loggedIn$: Observable<boolean>;

    constructor(
        private http: Http,
        @Inject(APP_CONFIG) config: AppConfig
    ) {

        this.config = config;

        this.state = new BehaviorSubject<AuthStateModel>(this.initalState);
        this.state$ = this.state.asObservable();

        this.tokens$ = this.state.filter(state => !!state.authReady).map(state => state.tokens);
        this.profile$ = this.state.filter(state => !!state.authReady).map(state => state.profile);

        this.loggedIn$ = this.tokens$.map(tokens => !!tokens.id_token);
    }
    init(): Observable<AuthTokenModel> {
        return this.startupTokenRefresh();//.do(() => this.scheduleRefresh());
    }

    register(data: RegisterModel | any): Observable<Response> {
        const headers = new Headers({ 'Content-Type': 'application/x-www-form-urlencoded' });
        const options = new RequestOptions({ headers: headers });
        const params = new URLSearchParams();
        Object.keys(data).forEach(key => params.append(key, data[key]));
        return this.http
            .post(`${this.config.baseUrl}/connect/register`, params.toString(), options)
            .catch(res => Observable.throw(res.json()));
    }

    login(user: LoginModel): Observable<any> {
        return this.getTokens(user, 'password')
            .catch(res => Observable.throw(res.json()))
            //.do(res => this.scheduleRefresh())
            ;
    }

    logout(): void {
        this.updateState({ profile: null, tokens: null });
        if (this.refreshSubscription$) {
            this.refreshSubscription$.unsubscribe();
        }
        this.removeToken();
    }

    private storeToken(tokens: AuthTokenModel): void {
        const previousTokens = this.retrieveTokens();
        if (previousTokens != null && tokens.refresh_token == null) {
            tokens.refresh_token = previousTokens.refresh_token;
        }

        localStorage.setItem('auth-tokens', JSON.stringify(tokens));
    }

    private removeToken(): void {
        localStorage.removeItem('auth-tokens');
    }

    private updateState(newState: AuthStateModel): void {
        const previousState = this.state.getValue();
        this.state.next(Object.assign({}, previousState, newState));
    }

    private getTokens(data: RefreshGrantModel | LoginModel | any, grantType: string): Observable<Response> {

        const headers = new Headers({ 'Content-Type': 'application/x-www-form-urlencoded' });
        const options = new RequestOptions({ headers: headers });

        Object.assign(data, { grant_type: grantType, scope: this.config.scope, client_id: this.config.clientId });

        const params = new URLSearchParams();
        Object.keys(data).forEach(key => params.append(key, data[key]));

        return this.http.post(`${this.config.baseUrl}/connect/token`, params.toString(), options)
            .do(res => {
                const tokens: AuthTokenModel = res.json();
                if (!tokens || !tokens.id_token) return;

                const now = new Date();
                tokens.expiration_date = new Date(now.getTime() + tokens.expires_in * 1000).getTime().toString();

                const profile: ProfileModel = this.jwtHelper.decodeToken(tokens.id_token);

                this.storeToken(tokens);
                this.updateState({ authReady: true, tokens, profile });
            });
    }

    private startupTokenRefresh(): Observable<AuthTokenModel> {

        return Observable.of(this.retrieveTokens())
            .flatMap((tokens: AuthTokenModel) => {

                if (!tokens || !tokens.id_token) {
                    this.updateState({ authReady: true });
                    return Observable.throw('No token in Storage');
                }
                const profile: ProfileModel = this.jwtHelper.decodeToken(tokens.id_token);
                this.updateState({ tokens, profile });

                if (+tokens.expiration_date > new Date().getTime()) {
                    this.updateState({ authReady: true });
                }

                return this.refreshTokens();
            })
            .catch(error => {
                this.logout();
                this.updateState({ authReady: true });
                return Observable.throw(error);
            });
    }

    private scheduleRefresh(): void {
        this.refreshSubscription$ = this.tokens$
            .first()
            // refresh every half the total expiration time
            .flatMap(tokens => Observable.interval(tokens.expires_in / 2 * 1000))
            .flatMap(() => this.refreshTokens())
            .subscribe();
    }

    refreshTokens(): Observable<AuthTokenModel> {
        return this.state.first()
            .map(state => state.tokens || { refresh_token: '' })
            .flatMap(tokens => this.getTokens({ refresh_token: tokens.refresh_token }, 'refresh_token')
                .catch(error => Observable.throw('Session Expired'))
            );
    }
    private retrieveTokens(): AuthTokenModel | any {
        const tokensString = localStorage.getItem('auth-tokens');
        const tokensModel: AuthTokenModel = tokensString == null ? null : JSON.parse(tokensString);
        return tokensModel;
    }

}

