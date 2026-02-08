<template>
  <v-app id="inspire">
    <v-main>
      <v-container fluid fill-height>
        <v-layout align-center justify-center>
          <v-flex xs12 sm8 md4>
            <v-card class="elevation-12">
              <v-toolbar dark color="primary">
                <v-toolbar-title> ورود</v-toolbar-title>
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
                    autocomplete="on"
                    v-model="loginForm.userName"
                    prepend-icon="mdi-lock"
                    name="userName"
                    :rules="userNameRules"
                    label="نام کاربری "
                  ></v-text-field>

                  <v-text-field
                    v-show="!register"
                    id="password"
                    :type="show1 ? 'text' : 'password'"
                    @click:append="show1 = !show1"
                    :append-icon="show1 ? 'mdi-eye' : 'mdi-eye-off'"
                    autocomplete="off"
                    v-model="loginForm.password"
                    prepend-icon="mdi-lock"
                    name="password"
                    label="رمز عبور"
                  ></v-text-field>

                  <v-row>
                    <div class="mt-3"></div>
                  </v-row>
                  <v-row> </v-row>
                  <v-row>
                    <v-spacer></v-spacer>

                    <v-col cols="4">
                      <v-btn :loading="loading" type="submit" color="primary"
                        >ورود
                      </v-btn>
                    </v-col>
                  </v-row>
                </v-form>
              </v-card-text>
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="success" text @click="downloadApk">
                  <v-icon left>mdi-download</v-icon>
                  دانلود اپلیکیشن
                </v-btn>
                <v-spacer></v-spacer>
              </v-card-actions>
            </v-card>
          </v-flex>
        </v-layout>
      </v-container>
    </v-main>
  </v-app>
</template>

<script>
import request from "@/utils/request";
import { UserModule } from "@/store/modules/user";

// import { VueRecaptcha } from "vue-recaptcha";
import Vue from "vue";

export default {
  name: "CustomerLogin",
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
      userName: "",
      password: "",
    },
    userNameRules: [(v) => !!v || "نام کاربری الزامی میباشد"],
  }),
  methods: {
    downloadApk() {
      // Create a link that points directly to the backend
      const link = document.createElement("a");
      link.href = `${
        process.env.VUE_APP_API_BASE_URL || "https://localhost:7087"
      }/File.apk`;
      link.download = "File.apk";
      link.target = "_blank";
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    },
    async handleError() {
      alert("خطا");
    },
    async handleSuccess(response) {
      this.loginForm.loginToken = response;
      this.captchaHasError = false;
    },

    async handleLogin() {
      this.loading = false;
      // if (this.captchaHasError) return;
      this.validationForm();
      if (!this.valid) return;

      this.loading = true;
      try {
        request
          .post(`/authentication/customer-login`, this.loginForm)
          .then((a) => {
            const result = a.data.result;
            if (result.seller) {
              UserModule.Login({
                mobile: this.loginForm.userName,
                password: this.loginForm.password,
              });
              this.$router.push("/dashboard/sshkeys");
            } else {
              UserModule.CustomerLogin(result);
              this.$router.push("/customerInfo");
            }
          })
          .finally(() => {
            this.loading = false;
          });
        // this.$router.push("/verify-code/");

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
