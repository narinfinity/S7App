import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppModuleShared } from './app.shared.module';
import { AppComponent } from './components/app/app.component';
import { APP_CONFIG, useValue } from './app.config';

@NgModule({
    id: module.id,
    bootstrap: [ AppComponent ],
    imports: [
        BrowserModule,
        AppModuleShared
    ],
    providers: [
        { provide: APP_CONFIG, useFactory: useValue },
    ]
})
export class AppModule {
}