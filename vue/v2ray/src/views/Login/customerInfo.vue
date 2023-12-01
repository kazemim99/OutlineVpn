<template>
  <v-app id="inspire">
    <v-app-bar fixed app>
      <v-spacer></v-spacer>

      <!-- <v-btn icon @click="getNotif()">
        <v-icon>mdi-bell-ring</v-icon>
        <v-badge color="green" content="15"> </v-badge>
      </v-btn> -->
      <v-btn icon @click="logout">
        <v-icon>mdi-logout</v-icon>
      </v-btn>
    </v-app-bar>
    <v-main>
      <v-container>
        <v-row>
          <v-col cols="12">
            <v-card>
              <v-card-title>
                <h5>
                  کسانی که مشکل قطعی یا کندی سرور دارند میتوانند در این قسمت
                  سرور رو خود را تغییر دهند
                </h5>
              </v-card-title>
              <v-card-title>
                <v-select
                  v-model="serverId"
                  :items="servers"
                  item-value="id"
                  item-text="title"
                  label="ُسرور"
                  solo
                ></v-select>
              </v-card-title>

              <v-card-text>
                <v-list>
                  <v-list-item>
                    <v-list-item-content>
                      <v-btn
                        :loading="loading"
                        @click="changeServer"
                        color="primary"
                        >تغییر سرور</v-btn
                      >
                    </v-list-item-content>
                  </v-list-item>

                  <v-list-item>
                    <v-list-item-content>
                      <v-list-item-title>ادرس سرور : </v-list-item-title>
                      <b class="my-3" style="color: crimson">
                        {{ user.server }}</b
                      >
                      <v-spacer></v-spacer>
                      <h4 class="my-3">اموزش نحوه تغییر سرور :</h4>
                      <ul>
                        <li>
                          <p>
                            1. در برنامه NapersternetV ابتدا فیلتر شکن را قطع
                            کرده
                          </p>
                        </li>
                        <li><p>2. از پایین صفحه وارد قسمت کانفیگز شوید</p></li>
                        <li>
                          <p>3. دکمه مداد را زده تا فرم اطلاعات باز شود</p>
                        </li>
                        <li>
                          <p>
                            4. قسمت سوم ssh host(هاست) به
                            <b style="color: crimson"> {{ user.server }}</b>
                            <v-icon
                              medium
                              class="mr-2"
                              @click="copyToClipBoard(user.server)"
                            >
                              mdi-content-copy</v-icon
                            >
                            تغییر دهید
                          </p>
                        </li>
                        <li>
                          <p>5. دکمه ذخیره را زده و مجدد متصل شود</p>
                        </li>
                        <li>
                          <b
                            >در صورت کندی یا قطعی مجدد سرور را تغییر داده و
                            مراحل را تکرار کنید
                          </b>
                        </li>
                      </ul>
                    </v-list-item-content>
                  </v-list-item>
                  <v-list-item>
                    <v-list-item-content>
                      <v-list-item-title>نام کاربری :</v-list-item-title>
                      <b> {{ user.userName }}</b>
                    </v-list-item-content>
                  </v-list-item>
                  <v-list-item>
                    <v-list-item-content>
                      <v-list-item-title>رمز عبور :</v-list-item-title>
                      <b> {{ user.password }}</b>
                    </v-list-item-content>
                  </v-list-item>

                  <v-list-item>
                    <v-list-item-content>
                      <v-list-item-title> تاریخ انقضا :</v-list-item-title>
                      <b> {{ user.expireDate }}</b>
                    </v-list-item-content>
                  </v-list-item>
                </v-list>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
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
  name: "customerInfo",
  components: {
    // VueRecaptcha,
  },

  data: () => ({
    serverId: null,
    loading: false,
    servers: [],
    user: {
      userName: null,
      password: null,
      server: null,
      expireDate: null,
    },
  }),

  created() {
    if (UserModule.token === null || UserModule.token === "") {
      this.$router.push("/");
    }
    this.getInfo();
  },
  methods: {
    logout() {
      UserModule.ResetToken();
      this.$router.push("/");
    },
    copyToClipBoard(textToCopy) {
      if (navigator.clipboard && window.isSecureContext) {
        navigator.clipboard
          .writeText(textToCopy)
          .then(() => {
            console.log(textToCopy);
          })
          .catch(() => {
            alert("خطا در کپی");
          });
      } else {
        const textArea = document.createElement("textarea");
        textArea.value = textToCopy;

        // Move textarea out of the viewport so it's not visible
        textArea.style.position = "absolute";
        textArea.style.left = "-999999px";

        document.body.prepend(textArea);
        textArea.select();

        try {
          document.execCommand("copy");
        } catch (error) {
          console.error(error);
        } finally {
          textArea.remove();
        }
      }
    },
    changeServer() {
      this.loading = true;
      Vue.swal({
        title: "ایا برای تغییر سرور مطمئن  هستید",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "بله",
        cancelButtonText: "انصراف",
      }).then((result) => {
        if (result.isConfirmed) {
          request
            .put(`/v2server/change-server/${this.serverId}`, null)
            .then(() => {
              Vue.swal("", "سرور با موفقیت تغییر پیدا کرد", "success");
              this.getInfo();
              this.loading = false;
            });
        }
      });
    },
    async getInfo() {
      await request.get(`/v2Server/customer-info`).then((response) => {
        this.user = response.data.result;
        this.getServers();
      });
    },
    async getServers() {
      await request.get(`/v2Server/customer-servers`).then((response) => {
        this.servers = response.data.result;
      });
    },
  },
};
</script>