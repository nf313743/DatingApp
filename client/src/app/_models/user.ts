export interface User {
  userName: string;
  token: string;
  photoUrl: string | null;
  knownAs: string;
  gender: string;
  roles: string[];
}
