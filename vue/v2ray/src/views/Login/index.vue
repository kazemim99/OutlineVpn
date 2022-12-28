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
                    :rules="
                      register
                        ? confirmPasswordRules.concat(validatePassword2)
                        : []
                    "
                    prepend-icon="mdi-lock"
                    name="confirmPassword"
                    label="تکرار رمز عبور"
                  ></v-text-field>
                  <v-row>
                    <!-- <VueRecaptcha
                      :sitekey="siteKey"
                      :load-recaptcha-script="true"
                      @verify="handleSuccess"
                      @error="handleError"
                    ></VueRecaptcha> -->
                  </v-row>
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
                      <v-btn
                        :loading="loading && register"
                        v-on:click="registerShow"
                        color="success"
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

<script>
import { UserModule } from "@/store/modules/user";
// import { VueRecaptcha } from "vue-recaptcha";

export default {
  name: "Login",
  components: {},

  data: () => ({
    siteKey: "6LcdGLUjAAAAAPqmwHQH5YB1siI6vEgddeqsTOtY",
    valid: false,
    loading: false,
    show1: false,
    show2: false,
    register: false,
    loginForm: {
      email: "",
      password: "",
      confirmPassword: "",
    },

    userNameRules: [
      (v) => !!v || "نام کاربری الزامی میباشد",
      (v) =>
        /[a-zA-Z0-9]{0,}([.]?[a-zA-Z0-9]{1,})[@](gmail.com|outlook.com|hotmail.com|yahoo.com)/.test(
          v
        ) ||
        "ایمیل وارد شده  اشتباه است ایمیلهای مورد تایید gmail,outlook,hotmail,yahoo",
    ],
    passwordRules: [
      (v) => !!v || "رمز عبور   الزامی میباشد",
      (v) => v.length > 7 || "رمز عبور   باید هشت رقم باشد ",
    ],
    confirmPasswordRules: [
      (value) => !!value || "لطفا تکرار رمز عبور را وارد نمایید",
    ],
  }),
  methods: {
    validatePassword2(value) {
      return value == this.loginForm.password || "تکرار رمز عبور اشتباه است";
    },
    async handleError() {
      console.log("b");
      // Do some validation
    },
    async handleSuccess(response) {
      console.log("a");
      // Do some validation
    },
    async registerShow() {
      this.loginForm.password = "";
      this.loginForm.confirmPassword = "";
      this.register = true;

      this.validationForm();
      if (!this.valid) return;

      this.handleRegister();
    },

    async loginShow() {
      debugger;
      this.loginForm.password = "";
      this.loginForm.confirmPassword = "";
      this.register = false;
    },

    async handleRegister() {
      debugger;
      if (!this.register) {
        this.registerShow();
      }
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
    },
    async handleLogin() {
      this.validationForm();
      if (!this.valid) return;

      this.loading = true;
      try {
        await UserModule.Login(this.loginForm);
        if (UserModule.needConfirm) {
          await UserModule.GetCode(this.loginForm.email);
          this.$router.push("/verify-code");
        } else {
          this.$router.push("/home");
        }
        this.loading = false;
      } catch (error) {
        this.loading = false;
      }
    },
    validationForm() {
      this.$refs.form.validate();
    },
  },
};
</script>