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
  customerLogin,
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
  public customerUserName = "";
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
  SET_Mobile(mobile: string) {
    this.mobile = mobile;
  }
  @Mutation

  @Mutation
  SET_FULLNAME(input: any) {
    this.fullName = `${input.firstName} ${input.lastName}`;
  }

  @Mutation
  Set_Customer_UserName(input: any) {
    this.customerUserName = `${input.UserName}`;
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
      this.ResetToken();
      const result = a.data.result;
      this.SET_Mobile(userInfo.mobile);
      const token = result.jwtToken.token;
      setToken(`Bearer ${token}`);
      store.commit("setUserDetails", result);

      this.SET_FULLNAME(result)
    });
  }

  @Action
  public async CustomerLogin(result: any) {
    this.ResetToken();
    const token = result.jwtToken.token;
    this.SET_TOKEN(token);
    setToken(`Bearer ${token}`);
    store.commit("setCustomerUserName", result.userName);
  }

  @Action
  public async Register(userInfo: { mobile: string; password: string }) {
    await register(userInfo).then((a) => {

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
  public async GetCode(input: { mobile: string, loginToken: string }) {
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
