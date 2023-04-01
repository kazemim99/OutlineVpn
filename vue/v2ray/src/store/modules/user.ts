import {
  VuexModule,
  Action,
  Mutation,
  getModule,
  Module,
} from "vuex-module-decorators";
import useRoute from "vue-router";

import store from "@/store";
import VueRouter from "vue-router";

import { getToken, setToken, removeToken } from "@/utils/cookies";
import {
  login,
  getCode,
  getUser,
  veriFyCode,
  changePassword,
  register,
} from "@/api/autheticationApi/users";

export interface IUserState {
  token: string;
  fullName: string;
  roles: string[];
  permisiones: string[];
}
@Module({ namespaced: true, dynamic: true, store, name: "auth" })
class User extends VuexModule implements IUserState {
  public token = getToken() || "";
  public fullName = "";
  public roles: string[] = [];
  public permisiones: string[] = [];
  public verfied = false;
  public needConfirm = false;
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
  SET_Mobile(mobile: string) {
    this.mobile = mobile;
  }
  @Mutation
  SET_NEEDCONFIRM(needConfirm: boolean) {
    this.needConfirm = needConfirm;
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
        this.SET_Mobile(verifyModel.mobile);
        const result = a.data.result;
        const token = result.jwtToken.token;
        setToken(`Bearer ${token}`);
        store.commit("setUserDetails", result);
      })
      .catch((e) => {
        this.SET_VERIFIED(false);
      });
  }

  @Action
  public async Login(userInfo: { mobile: string; password: string }) {
    await login(userInfo).then((a) => {
      const result = a.data.result;
      this.SET_Mobile(userInfo.mobile);
      this.SET_NEEDCONFIRM(result.needConfirm);
      if (!result.needConfirm) {
        const token = result.jwtToken.token;
        setToken(`Bearer ${token}`);
        store.commit("setUserDetails", result);
      }
    });
  }

  @Action
  public async Register(userInfo: { mobile: string; password: string }) {
    await register(userInfo).then((a) => {
      this.SET_Mobile(userInfo.mobile);
      // this.GetCode(userInfo.mobile)
    });
  }
  @Action
  public async GetUserDetails() {
    await getUser().then((a) => {
      const result = a.data.result;
      store.commit("setUserDetails", result);
    });
  }
  @Action
  public async GetCode(input: { mobile: string,loginToken: string }) {
    this.SET_Mobile(input.mobile);
    await getCode(input);
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
