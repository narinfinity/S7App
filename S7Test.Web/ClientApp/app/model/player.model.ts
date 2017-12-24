import { TeamModel } from './team.model';
export class PlayerModel {
    id = 0;
    isActive = false;
    picture = 'https://picsum.photos/32/32/?random';
    age = 0;
    yellowCards = 0;
    redCards = 0;
    goals = 0;
    appearances = 0;
    name = '';
    gender = '';
    team = new TeamModel();
}