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
                  <!-- <v-text-field
                    v-show="!register"
                    prepend-icon="mdi-account"
                    name="login"
                    v-model="loginForm.email"
                    label="نام کاربری  "
                    autocomplete="off"
                    :rules="userNameRules"
                    type="text"
                  >
                
                </v-text-field> -->

                  <v-text-field
                    id="شماره موبایل"
                    autocomplete="on"
                    v-model="loginForm.mobile"
                    prepend-icon="mdi-lock"
                    name="mobile"
                    :rules="userNameRules"
                    label="شماره موبایل"
                  ></v-text-field>

                  <v-text-field
                    v-show="!register"
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

                  <!-- <v-text-field
                    id="شماره موبایل"
                    autocomplete="on"
                    v-model="loginForm.mobile"
                    prepend-icon="mdi-lock"
                    name="mobile"
                    :rules="userNameRules"
                    label="شماره موبایل"
                  ></v-text-field> -->
                  <v-row>
                    <div class="mt-3">
                      <!-- <VueRecaptcha
                        :sitekey="siteKey"
                        :load-recaptcha-script="true"
                        @verify="handleSuccess"
                        @error="handleError"
                      ></VueRecaptcha> -->
                    </div>
                  </v-row>
                  <v-row>
                    <!-- <v-col cols="4">
                      <router-link to="/get-code" class="d-flex justify-end"
                        >فراموشی رمز ؟</router-link
                      >
                    </v-col> -->
                  </v-row>
                  <v-row>
                    <v-spacer></v-spacer>

                    <!-- <v-col cols="3">
                      <v-btn
                        :loading="loading && register"
                        v-on:click="registerShow"
                        color="success"
                        >ثبت نام</v-btn
                      >
                    </v-col> -->
                    <v-col cols="4">
                      <v-btn
                        :loading="loading"
                        v-show="!register"
                        type="submit"
                        color="primary"
                        >ورود</v-btn
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
import Vue from "vue";

export default {
  name: "Login",
  components: {
    // VueRecaptcha,
  },
  metaInfo: {
    meta: [
      {
        name: "description",
        content:
          "فروش فیلتر شکن پر سرعت برای اندرودید , ایفون , لینوکس , اندروید , ویندوز و کامپیوتر",
      },
      { property: "og:title", content: "ایران وی توی ری : فروش فیلتر شکن" },
    ],
  },
  data: () => ({
    siteKey: "6LfYMdwjAAAAACbHDborqW_pxSS3z2Gnm6_CqE-Y",
    captchaHasError: true,
    valid: false,
    loading: false,
    show1: false,
    show2: false,
    register: false,
    loginForm: {
      mobile: "",
      password: "",
    },
    userNameRules: [
      (v) => !!v || "نام کاربری الزامی میباشد",
      (v) => /^(09|9)+([0-9]){9}$/.test(v) || "شماره موبایل اشتباه است",
    ],
  }),
  methods: {
    validatePassword2(value) {
      return value == this.loginForm.password || "تکرار رمز عبور اشتباه است";
    },
    async handleError() {
      alert("خطا");
    },
    async handleSuccess(response) {
      this.loginForm.loginToken = response;
      this.captchaHasError = false;
    },

    async handleRegister() {
      if (this.captchaHasError) return;
      if (!this.register) {
        this.registerShow();
      }
      this.validationForm();
      if (!this.valid) return;

      this.loading = true;
      try {
        this.loginForm.loginToken = "12";
        await UserModule.Register(this.loginForm);

        this.loading = false;
        this.$router.push("/verify-code");
      } catch (error) {
        this.loading = false;
      }
    },
    async handleLogin() {
      // if (this.captchaHasError) return;
      this.validationForm();
      if (!this.valid) return;

      this.loading = true;
      try {
        await UserModule.Login(this.loginForm);
        // this.$router.push("/verify-code/");
        this.$router.push("/dashboard/sshkeys");

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
