<template>
  <v-app id="inspire">
    <v-main>
      <v-container fluid fill-height>
        <v-layout align-center justify-center>
          <v-flex xs12 sm8 md4>
            <v-card class="elevation-12">
              <v-toolbar dark color="primary">
                <v-toolbar-title>تغییر رمز عبور</v-toolbar-title>
              </v-toolbar>
              <v-card-text>
                <v-form
                  @submit.prevent="changePassword"
                  v-model="valid"
                  ref="form"
                >
                  <v-text-field
                    id="password"
                    :type="show1 ? 'text' : 'password'"
                    @click:append="show1 = !show1"
                    :append-icon="show1 ? 'mdi-eye' : 'mdi-eye-off'"
                    :rules="passwordRules"
                    autocomplete="off"
                    v-model="password"
                    prepend-icon="mdi-lock"
                    name="password"
                    label="رمز عبور"
                  ></v-text-field>
                  <v-text-field
                    id="confirm-password"
                    :type="show1 ? 'text' : 'password'"
                    @click:append="show1 = !show1"
                    :append-icon="show1 ? 'mdi-eye' : 'mdi-eye-off'"
                    :rules="[confirmPasswordRules, compareConfirmPasswordRules]"
                    autocomplete="off"
                    v-model="confirmPassword"
                    prepend-icon="mdi-lock"
                    name="confirmpassword"
                    label="تکرار رمز عبور"
                  ></v-text-field>
                  <v-row>
                    <v-col cols="3">
                      <router-link to="/get-code" class="d-flex justify-end"
                        >دریافت مجدد کد</router-link
                      >
                    </v-col>
                    <v-spacer></v-spacer>
                    <v-col cols="2">
                      <v-btn :loading="loading" type="submit" color="primary"
                        >تغییر</v-btn
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
  private password = "";
  private confirmPassword = "";

  private passwordRules = [
    (v: string) => !!v || "رمز عبور   الزامی میباشد",
    (v: string) => v.length > 5 || "رمز عبور   باید 6 رقم باشد ",
  ];

  private confirmPasswordRules = [
    (v: string) => !!v || "لطفا تکرار رمز عبور را وارد نمایید",
  ];

  get compareConfirmPasswordRules() {
    return (
      this.password === this.confirmPassword || "تکرار رمز عبور اشتباه است"
    );
  }
  private async changePassword() {
    this.validationForm();
    if (!this.valid) return;

    this.loading = true;
    try {
      await UserModule.ChangePassword({
        password: this.password,
        confirmPassword: this.confirmPassword,
      });
      this.loading = false;
      this.$router.push("/login");
    } catch (error) {
      this.loading = false;
    }
  }
  private validationForm() {
    this.$refs.form.validate();
  }
}
</script>
