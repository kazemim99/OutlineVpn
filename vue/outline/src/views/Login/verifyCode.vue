<template>
  <v-app id="inspire">
    <v-main>
      <v-container fluid fill-height>
        <v-layout align-center justify-center>
          <v-flex xs12 sm8 md4>
            <v-card class="elevation-12">
              <v-toolbar dark color="primary">
                <v-toolbar-title>کد تایید</v-toolbar-title>
              </v-toolbar>
              <v-card-text>
                <v-text-field
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
                      @click="verifyCode()"
                      color="primary"
                    >
                      ارسال</v-btn
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
  name: "VerifyCode",
  components: {},
})
export default class extends Vue {
  $refs!: {
    form: HTMLFormElement;
  };
  private valid = false;
  private codeSent = false;
  private loading = false;
  private mobile = "";
  private code = "";

  private async verifyCode() {
    this.loading = true;
    try {
      if (this.code.length != 4) {
        alert("کد اشتباه است");
        return;
      }
      await UserModule.VerifyCode({ code: this.code, mobile: UserModule.mobile });
      this.loading = false;
      if (UserModule.verfied)
        if (UserModule.isAdmin) this.$router.push("/");
        else this.$router.push("/plans");
    } catch (error) {
      this.loading = false;
    }
  }
  private validationForm() {
    this.$refs.form.validate();
  }
}
</script>
