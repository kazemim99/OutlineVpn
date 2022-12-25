<template>
  <v-app id="inspire">
    <v-main>
      <v-container fluid fill-height>
        <v-layout align-center justify-center>
          <v-flex xs12 sm8 md4>
            <v-card class="elevation-12">
              <v-toolbar dark color="primary">
                <v-toolbar-title>فرم ورود</v-toolbar-title>
              </v-toolbar>
              <v-card-text>
                <v-form
                  @submit.prevent="handleLogin"
                  v-model="valid"
                  ref="form"
                >
                  <v-text-field
                    prepend-icon="mdi-account"
                    name="login"
                    v-model="loginForm.email"
                    label="ایمیل  "
                    autocomplete="off"
                    :rules="userNameRules"
                    type="text"
                  ></v-text-field>
                  <v-text-field
                    id="password"
                    :type="show1 ? 'text' : 'password'"
                    @click:append="show1 = !show1"
                    :append-icon="show1 ? 'mdi-eye' : 'mdi-eye-off'"
                    :rules="passwordRules"
                    autocomplete="off"
                    v-model="loginForm.password"
                    prepend-icon="mdi-lock"
                    name="password"
                    label="رمز عبور"
                  ></v-text-field>

                  <v-text-field
                    v-show="register"
                    id="Confirmpassword"
                    :type="show2 ? 'text' : 'password'"
                    @click:append="show2 = !show2"
                    :append-icon="show2 ? 'mdi-eye' : 'mdi-eye-off'"
                    autocomplete="off"
                    v-model="loginForm.confirmPassword"
                    prepend-icon="mdi-lock"
                    name="confirmPassword"
                    label="تکرار رمز عبور"
                  ></v-text-field>

                  <v-row>
                    <v-spacer></v-spacer>
                    <v-col cols="4">
                      <v-btn
                        :loading="loading"
                        v-show="!register"
                        type="submit"
                        color="primary"
                        >ورود</v-btn
                      >

                      <v-btn
                        v-show="register"
                        v-on:click="loginShow()"
                        color="primary"
                        >انصراف</v-btn
                      >
                    </v-col>

                    <v-col cols="4">
                      <v-btn v-on:click="registerShow()" color="success"
                        >ثبت نام</v-btn
                      >
                    </v-col>
                  </v-row>
                </v-form>
              </v-card-text>
              <v-card-actions> </v-card-actions>
            </v-card>
          </v-flex>
        </v-layout>
      </v-container>
    </v-main>
  </v-app>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { UserModule } from "@/store/modules/user";

@Component({
  name: "Login",
  components: {},
})
export default class extends Vue {
  $refs!: {
    form: HTMLFormElement;
  };
  private valid = false;
  private loading = false;
  private show1 = false;
  private show2 = false;
  private register = false;
  private loginForm = {
    email: "",
    password: "",
  };

  private userNameRules = [
    (v: string) => !!v || "نام کاربری الزامی میباشد",
    (v: string) =>
      /[a-zA-Z0-9]{0,}([.]?[a-zA-Z0-9]{1,})[@](gmail.com|outlook.com|hotmail.com|yahoo.com)/.test(
        v
      ) ||
      "ایمیل وارد شده  اشتباه است ایمیلهای مورد تایید gmail,outlook,hotmail,yahoo",
  ];
  private passwordRules = [
    (v: string) => !!v || "رمز عبور   الزامی میباشد",
    (v: string) => v.length > 7 || "رمز عبور   باید هشت رقم باشد ",
  ];
  private confirmPasswordRules = [
    (value) =>
      !!value || ("لطفا تکرار رمز عبور را وارد نمایید" && this.register),
    (value) =>
      (value === this.loginForm.password && this.register) ||
      "تکرار رمز عبور اشتباه است",
  ];
  private registerShow() {
    this.register = true;

    this.validationForm();
    if (!this.valid) return;

    this.handleRegister();
  }

  private loginShow() {
    this.register = false;
  }
  private async handleRegister() {
    this.validationForm();
    if (!this.valid) return;

    this.loading = true;
    try {
      await UserModule.Register(this.loginForm);

      this.loading = false;
      this.$router.push("/verify-code");
    } catch (error) {
      this.loading = false;
    }
  }
  private async handleLogin() {
    this.validationForm();
    if (!this.valid) return;

    this.loading = true;
    try {
      await UserModule.Login(this.loginForm);
      this.loading = false;
      if (UserModule.needConfirm) {
        await UserModule.GetCode(this.loginForm.email);
        this.$router.push("/verify-code");
      } else {
        debugger;
        this.$router.push("/home");
      }
    } catch (error) {
      this.loading = false;
    }
  }
  private validationForm() {
    this.$refs.form.validate();
  }
}
</script>
