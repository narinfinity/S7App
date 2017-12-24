import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { AuthService } from '../../service/auth.service';
import { AuthStateModel } from '../../model/auth.state.model';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent implements OnInit {
    authState$: Observable<AuthStateModel>;

    constructor(
        private authService: AuthService,
    ) { }

    ngOnInit() {
        this.authState$ = this.authService.state$;
        // This starts up the token refresh preocess for the app
        this.authService.init()
            .subscribe(
            () => { console.info('Startup success'); },
            error => console.warn(error)
            );
    }
    refreshToken() {
        this.authService.refreshTokens()
            .subscribe();
    }
    logout() {
        this.authService.logout();
    }
}
