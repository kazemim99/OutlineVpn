import {
  VuexModule,
  Action,
  Mutation,
  getModule,
  Module,
} from "vuex-module-decorators";
import store from "@/store";
import { getToken, setToken, removeToken } from "@/utils/cookies";
import {
  login,
  getCode,
  veriFyCode,
  changePassword,
} from "@/api/autheticationApi/users";

export interface IUserState {
  token: string;
  fullName: string;
  roles: string[];
  permisiones: string[];
}

@Module({ namespaced: true, dynamic: true, store, name: "user" })
class User extends VuexModule implements IUserState {
  public token = getToken() || "";
  public fullName = "";
  public roles: string[] = [];
  public permisiones: string[] = [];
  public verfied = false;
  public mobile = "";
  public isAdmin = false;
  @Mutation
  SET_TOKEN(token: string) {
    this.token = token;
  }
  @Mutation
  SET_ROLES(roles: string[]) {
    this.roles = roles;
  }

  @Mutation
  SET_VERIFIED(result: boolean) {
    this.verfied = result;
  }

  @Mutation
  SET_MOBILE(mobile: string) {
    this.mobile = mobile;
  }
  @Mutation
  SET_FULLNAME(input: any) {
    this.fullName = `${input.firstName} ${input.lastName}`;
  }

  @Mutation
  SET_IsAdmin(isAdmin: any) {
    this.isAdmin = isAdmin;
  }

  @Action
  public async VerifyCode(verifyModel: { code: string; mobile: string }) {
    await veriFyCode(verifyModel)
      .then((a) => {
        this.SET_VERIFIED(true);
        this.SET_MOBILE(verifyModel.mobile);
      })
      .catch(() => {
        this.SET_VERIFIED(false);
      });
  }

  @Action
  public async Login(userInfo: { username: string; password: string }) {
    await login(userInfo).then((a) => {
      const result = a.data.result;
      const token = result.jwtToken.token;
      localStorage.setItem(
        "permissions",
        JSON.stringify(result.permissions)
      );
      setToken(`Bearer ${token}`);
      this.SET_FULLNAME(result);
      this.SET_IsAdmin(result.isAdmin);
      this.SET_MOBILE(result.userName);
      store.commit("setUserDetails", result);
    });
  }

  @Action
  public async GetCode(mobile: string) {
    await getCode(mobile);
  }

  @Action
  public async ChangePassword(input: {
    password: string;
    confirmPassword: string;
  }) {
    await changePassword(this.mobile, input);
  }
  @Action
  public ResetToken() {
    removeToken();
    this.SET_TOKEN("");
    this.SET_ROLES([]);
  }
}

export const UserModule = getModule(User);
