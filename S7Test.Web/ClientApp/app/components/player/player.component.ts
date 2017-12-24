import { Component, Inject, OnInit } from '@angular/core';
import { PlayerService } from '../../service/player.service';
import { PlayerModel } from '../../model/player.model';
import { TeamModel } from '../../model/team.model';

import { AppConfig, APP_CONFIG } from '../../../app/app.config';

@Component({
    selector: 'app-player',
    templateUrl: './player.component.html'
})
export class PlayerListComponent implements OnInit {
    public addOrEditModel: PlayerModel;
    public response: any;

    public players: PlayerModel[];
    public isAddOrEditMode: boolean = false;

    public filterState: any;
    public pages: number[];
    public pageSizes: number[];

    constructor(
        private playerService: PlayerService,
        @Inject(APP_CONFIG) config: AppConfig
    ) {
        this.filterState = config.filterState;
        this.pageSizes = config.pageSizes;
    }
    ngOnInit() {

        this.getPlayerList(this.filterState);
    }

    getPlayerList(args: any) {
        this.playerService.getPlayers(args)
            .subscribe(res => {
                if (!(res = res.json())) return;

                this.players = res.data as PlayerModel[];
                this.pages = Array(res.pageCount).fill(0).map((e, i) => i + 1);
            }, error => {
                this.response = "Sign in to see the palayers' list";
            });
    }

    onHeaderClick(e: Event) {

        const col = (e.target as HTMLTableHeaderCellElement).getAttribute("id");
        if (!col || col == "picture") return;
        if (this.filterState.orderBy.prop !== col) this.filterState.orderBy = { prop: col, asc: true };
        else if (this.filterState.orderBy.asc == null) this.filterState.orderBy.asc = true;
        else if (this.filterState.orderBy.asc == true) this.filterState.orderBy.asc = false;
        else if (this.filterState.orderBy.asc == false) this.filterState.orderBy.asc = null;

        this.getPlayerList(this.filterState);
    }

    onPageSizeSelect(pageSize: number, e: Event) {
        this.filterState.pageSize = pageSize;
        this.filterState.page = 1;
        this.getPlayerList(this.filterState);
    }

    onSearch(e: Event) {
        this.filterState.keyword = (e.target as HTMLInputElement).value;
        this.filterState.page = 1;
        this.getPlayerList(this.filterState);
    }

    onPageChange(page: number, prevOrNext: number) {

        if (prevOrNext) {
            page = this.filterState.page + prevOrNext;
            this.filterState.page = page < 1 ? 1 : page > this.pages.length ? this.pages.length : page;
        }
        else this.filterState.page = page;

        this.getPlayerList(this.filterState);
    }

    onAddOrEditOpen(model = new PlayerModel()) {
        this.addOrEditModel = model;
        this.isAddOrEditMode = true;
    }
    onAddEditClose(close: boolean) { this.isAddOrEditMode = close; }

    onModelSaved() {
        this.getPlayerList(this.filterState);
        this.isAddOrEditMode = false;
    }

    deletePlayer(playerId: number) {

        this.playerService.deletePlayer(playerId)
            .subscribe(res => {
                if (!(res = res.json())) return;

                if (res.success) {                    
                    this.getPlayerList(this.filterState);
                    this.response = null;
                }
                else this.response = res.errors;
            }, error => {
                this.response = "An error occured, please try agin.";
            });
    }

}

