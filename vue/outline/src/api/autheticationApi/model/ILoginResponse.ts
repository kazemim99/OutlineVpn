export interface ILoginResponse {}

export interface ILoginResultDto {
  JwtToken: IJwtToken;

  RefreshToken: IRefreshToken;

  UserName: string;

  FirstName: string;

  LastName: string;

  Id: number;
}
export interface IJwtToken {
  Token: string;

  Expire: Date;

  RefreshToken: IRefreshToken;
}

export interface IRefreshToken {
  UserName: string;

  TokenString: string;

  ExpireAt: Date;

  UserId: number;
}

export interface IApiResponse<T> {
  Data: T;
}
