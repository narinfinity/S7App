import { Component, OnInit, Inject, Injectable } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../service/auth.service';

@Component({
    selector: 'app-register',
    templateUrl: './register.template.html'
})
export class RegisterComponent implements OnInit {
    registerForm: FormGroup;
    response: any;
    private formSubmitAttempt: boolean;

    constructor(
        private formBuilder: FormBuilder,
        private authService: AuthService,
        private router: Router,
    ) { }

    ngOnInit() {
        this.registerForm = this.formBuilder.group({
            firstName: ['FirstName', [Validators.required]],
            lastName: ['LastName', [Validators.required]],
            email: ['test@test.com', [Validators.required, Validators.email]],
            userName: ['testuser', [Validators.required]],
            password: ['Test@123', [Validators.required]],
            confirmPassword: ['Test@123', [Validators.required]]
        });
    }

    onSubmit() {
        if (this.registerForm.valid) {
            this.authService.register(this.registerForm.value)
                .subscribe((res) => {
                    const result = res.json();
                    if (!result.errors.length) this.router.navigate(['/login']);

                    //this.reset();
                    this.response = result.errors;
                },
                error => this.response = error);
        } else {
            this.validateAllFormFields(this.registerForm);
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
        const control = this.registerForm.get(field);
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
        this.registerForm.reset();
    }
}
