import { Component, Inject, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { PlayerModel } from '../../../model/player.model';
import { TeamModel } from '../../../model/team.model';
import { AppConfig, APP_CONFIG } from '../../../../app/app.config';
import { PlayerService } from '../../../service/player.service';

@Component({
    selector: 'app-addplayer',
    templateUrl: './add.player.component.html',
    styleUrls: ['./add.player.component.css']
})
export class AddPlayerComponent implements OnInit {
    @Input() model: PlayerModel;

    @Output() saved = new EventEmitter();
    @Output() close = new EventEmitter<boolean>();

    public response: any;
    public addForm: FormGroup;

    public teams: TeamModel[];
    public genders: string[] = ['Male', 'Female'];

    public appConfig: AppConfig;

    constructor(
        private playerService: PlayerService,
        private formBuilder: FormBuilder,
        @Inject(APP_CONFIG) config: AppConfig
    ) {
        this.appConfig = config;

    }
    ngOnInit() {
        const m = this.model;
        
        this.addForm = this.formBuilder.group({
            picture: [m.picture, [Validators.required, Validators.maxLength(2048)]],
            name: [m.name, [Validators.required, Validators.maxLength(1024)]],
            isActive: [m.isActive, [Validators.required]],
            gender: [m.gender, [Validators.required]],
            age: [m.age, [Validators.required, Validators.min(0)]],
            yellowCards: [m.yellowCards, [Validators.required, Validators.min(0)]],
            redCards: [m.redCards, [Validators.required, Validators.min(0)]],
            goals: [m.goals, [Validators.required, Validators.min(0)]],
            appearances: [m.appearances, [Validators.required, Validators.min(0)]],
            teamid: [m.team.id, [Validators.required]],
        });
        if (!this.teams) this.getTeams();
    }
    onCancel() {
        this.close.emit(false);
    }    
    onSubmit(e: Event) {
        this.isFormSubmitted = true;

        if (this.addForm.valid) {

            const formModel = this.addForm.value;
            formModel.id = this.model.id;

            const tmpPlayer: any = {};
            Object.keys(formModel).forEach(k => { if (k !== 'teamid') tmpPlayer[k] = formModel[k]; });
            tmpPlayer.team = this.teams.map(t => { return { id: t.id, name: t.name }; }).find(t => t.id == formModel['teamid']);
            
            this.model = Object.assign(this.model, tmpPlayer);
           
            this.playerService.addOrUpdatePlayer(this.model)
                .subscribe(res => {
                    if (!(res = res.json())) return;
                    if (res.success) { 
                        this.saved.emit();
                        this.response = null;
                        this.onCancel();
                        this.resetForm();
                    }
                    else this.response = res.errors;
                }, error => {
                    this.response = "An error occured, please try agin.";
                });
        } else {
            this.validateAllFormFields(this.addForm);
        }

    }
    getTeams() {
        this.playerService.getTeams({})
            .subscribe((res: any) => {
                if (!(res = res.json())) return;

                this.teams = res.map((e: any) => { return { id: e.id, name: e.name }; }) as TeamModel[];

            }, (error: any) => {
                this.onCancel();
                this.response = "Sign in to see the palayers' list";
            });
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

    isFormSubmitted: boolean = false;
    isFieldValid(field: string, error: string): boolean {
        const control: any = this.addForm.get(field);
        if (!control) return false;

        if (!this.isFormSubmitted) return false;
        else
            return error && control.errors
                ? control.errors[error]
                : control.invalid && (control.dirty || control.touched);
    }

    resetForm() {
        this.addForm.reset();
        this.isFormSubmitted = false;
    }
}

