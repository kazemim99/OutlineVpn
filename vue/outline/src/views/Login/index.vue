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
                    v-model="username"
                    label="شماره موبایل"
                    autocomplete="off"
                    :rules="userNameRules"
                    type="text"
                  ></v-text-field>
                  <v-row>
                    <v-spacer></v-spacer>
                    <v-col cols="4">
                      <v-btn :loading="loading" type="submit" color="primary"
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
private username ="";


  private userNameRules = [
    (v: string) => /^(09|9)+([0-9]){9}$/.test(v) || "شماره موبایل اشتباه است",
  ];

  private async handleLogin() {
    this.validationForm();
    if (!this.valid) return;

    this.loading = true;
    try {
      await UserModule.Login(this.username);
      await UserModule.GetCode(this.username);

      this.loading = false;
      this.$router.push("/verify-code");

    } catch (error) {
      this.loading = false;
    }
  }
  private validationForm() {
    this.$refs.form.validate();
  }
}
</script>
