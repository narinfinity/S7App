import { Component, OnInit, Input } from '@angular/core';

@Component({
    selector: 'app-field-error',
    templateUrl: './field.error.component.html'    
})
export class FieldErrorComponent {

    @Input() errorMsg: string;
    @Input() displayError: boolean;

}