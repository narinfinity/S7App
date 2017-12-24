import 'reflect-metadata';
import 'zone.js';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/first';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/filter';

import 'rxjs/add/observable/of';
import 'rxjs/add/observable/interval';
import 'rxjs/add/observable/throw';
import { NgModule, ErrorHandler } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { PlayerListComponent } from './components/player/player.component';
import { AddPlayerComponent } from './components/player/add/add.player.component';
import { FieldErrorComponent } from './components/fielderror/field.error.component';

import { AuthService } from './service/auth.service';
import { PlayerService } from './service/player.service';
import { AppConfig, APP_CONFIG, useValue } from '../app/app.config';

const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },

    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },

    { path: 'players', component: PlayerListComponent },
    { path: '**', redirectTo: 'login' }
];

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,

        LoginComponent,
        RegisterComponent,
        PlayerListComponent,
        AddPlayerComponent,
        FieldErrorComponent
    ],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot(routes)
    ],
    providers: [
        { provide: AuthService, useClass: AuthService },
        { provide: PlayerService, useClass: PlayerService },
        { provide: APP_CONFIG, useFactory: useValue },
    ]
})
export class AppModuleShared {
}
