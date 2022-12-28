<template>
  <v-app id="inspire">
    <v-main>
      <v-container fluid fill-height>
        <v-layout align-center justify-center>
          <v-flex xs12 sm8 md4>
            <v-card class="elevation-12">
              <v-toolbar dark color="primary">
                <v-toolbar-title>فراموشی رمز</v-toolbar-title>
              </v-toolbar>
              <v-card-text>
                <v-text-field
                  prepend-icon="mdi-account"
                  name="login"
                  v-model="email"
                  label="ایمیل"
                  autocomplete="off"
                  :rules="userNameRules"
                  type="text"
                ></v-text-field>
                <v-text-field
                  :disabled="!codeSent"
                  id="code"
                  autocomplete="off"
                  v-model="code"
                  prepend-icon="mdi-numeric "
                  name="code"
                  label="کد تایید"
                ></v-text-field>
                <v-row>
                  <v-col cols="4">
                    <router-link to="/login" class="d-flex justify-end"
                      >صفحه ورود</router-link
                    >
                  </v-col>
                  <v-spacer></v-spacer>
                  <v-col cols="4">
                    <v-btn
                      :loading="loading"
                      @click="!codeSent ? getCode() : verifyCode()"
                      color="primary"
                      >{{ !codeSent ? "دریافت کد" : "ارسال" }}</v-btn
                    >
                  </v-col>
                </v-row>
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
  private codeSent = false;
  private loading = false;
  private show1 = false;
  private email = "";
  private code = "";

  private userNameRules = [
    (v: string) => !!v || "نام کاربری الزامی میباشد",
    (v: string) => /^(09|9)+([0-9]){9}$/.test(v) || "شماره موبایل اشتباه است",
  ];

  private async getCode() {
    this.loading = true;
    try {
      await UserModule.GetCode(this.email);
      this.loading = false;
      this.codeSent = true;
      // this.$router.push("/change-password");
    } catch (error) {
      this.loading = false;
    }
  }
  private async verifyCode() {
    this.loading = true;
    try {
      await UserModule.VerifyCode({ code: this.code, email: this.email });
      this.loading = false;
      if (UserModule.verfied) this.$router.push("/change-password");
    } catch (error) {
      this.loading = false;
    }
  }
  private validationForm() {
    this.$refs.form.validate();
  }
}
</script>
