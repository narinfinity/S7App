import { AuthTokenModel } from './auth.tokens.model';
import { ProfileModel } from './profile.model';

export interface AuthStateModel {
  tokens?: AuthTokenModel | null;
  profile?: ProfileModel | null;
  authReady?: boolean;
}
