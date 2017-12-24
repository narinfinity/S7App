import { Component, OnInit, Inject, Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthService } from '../../service/auth.service';


@Component({
    selector: 'app-login',
    templateUrl: './login.template.html'
})
export class LoginComponent implements OnInit {
    loginForm: FormGroup;
    response: any;

    constructor(
        private formBuilder: FormBuilder,
        private authService: AuthService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit(): void {
        this.loginForm = this.formBuilder.group({
            username: ['testuser', [Validators.required]],
            password: ['Test@123', [Validators.required]],
        });        
    }

    onSubmit() {
        if (this.loginForm.valid) {
        this.authService.login(this.loginForm.value)
            .subscribe((res) => {
                this.reset();
                this.response = 'Successfully loggedin';
                this.router.navigate(["/players"]);
            },
            error => this.response = error);
        } else {
            this.validateAllFormFields(this.loginForm);
        }

    }
    private validateAllFormFields(formGroup: FormGroup) {
        Object.keys(formGroup.controls).forEach(field => {
            const control = formGroup.get(field);
            if (control instanceof FormControl) {
                control.markAsTouched({ onlySelf: true });
            } else if (control instanceof FormGroup) {
                this.validateAllFormFields(control);
            }
        });
    }

    isFieldValid(field: string): boolean {
        const control = this.loginForm.get(field);
        if (!control) return false;
        return control.invalid && (control.dirty || control.touched);
    }
    displayFieldCss(field: string) {
        return {
            'has-error': this.isFieldValid(field),
            //'has-feedback': this.isFieldValid(field)
        };
    }
    reset() {
        this.loginForm.reset();
    }

}
