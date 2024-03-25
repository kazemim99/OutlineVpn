<template>
  <v-row justify="center">
    <v-dialog
      v-model="dialog"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
      persistent
      max-width="600px"
    >
      <template v-slot:activator="{ on, attrs }">
        <v-btn color="primary" dark v-bind="attrs" v-on="on">ثبت کاربر</v-btn>
      </template>
      <v-card>
        <v-card-title>
          <span class="text-h5">ثبت کاربر جدید</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container>
              <v-row>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    v-model="user.firstName"
                    label="نام *"
                    placeholder=" "
                    autocomplete="false"
                    :rules="FirstNameRules"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    autocomplete="false"
                    v-model="user.lastName"
                    label="نام خانوادگی *"
                    :rules="LastNameRules"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    v-model="user.mobile"
                    autocomplete="false"
                    label=" موبایل *"
                    :rules="MobileRules"
                    required
                  ></v-text-field>
                </v-col>
              </v-row>
       
              <v-row>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    v-model="user.oneAccountPrice"
                    autocomplete="false"
                    label="قیمت تک کاربر"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    v-model="user.twoAccountPrice"
                    autocomplete="false"
                    label="قیمت دو کاربر"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    v-model="user.threeAccountPrice"
                    autocomplete="false"
                    label="قیمت سه کاربره"
                    required
                  ></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    v-model="user.password"
                    autocomplete="new-password"
                    name="password"
                    placeholder=" "
                    label="رمز عبور *"
                    :rules="[passwordRules]"
                    persistent-placeholder
                    type="password"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    v-model="user.confirmPassword"
                    autocomplete="off"
                    name="confirmPassword"
                    label="تکرار رمز عبور *"
                    persistent-placeholder
                    :rules="[passwordConfirmationRule]"
                    type="password"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    v-model="user.accountLimit"
                    autocomplete="false"
                    label=" محدودیت اکانت"
                    required
                  ></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="4">
                  <v-switch
                    v-model="user.enable"
                    :label="`وضعیت: ${user.enable ? 'فعال' : 'غیر فعال'}`"
                  ></v-switch>
                </v-col>

                <v-col cols="4" v-if="this.$store.state.userDetails.isAdmin">
                  <v-switch
                    v-model="user.isAdmin"
                    :label="`نقش: ${user.isAdmin ? 'ادمین' : 'کاربر عادی'}`"
                  ></v-switch>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click="dialog = false">بستن</v-btn>
          <v-btn :loading="loading" color="blue darken-1" text @click="submit()"
            >ذخیره</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script>
import Vue from "vue";
import request from "@/utils/request";
export default Vue.extend({
  name: "AddNewUser",
  components: {},
  props: ["selectedComplexId"],
  data: () => ({
    userId: null,
    hasImage: false,
    dialog: false,
    dialogLogo: false,
    valid: true,
    loading: false,
    imageUrl: "",
    servers: [],
    userRoles: [],
    user: {
      serverId: 0,
      initCapacity: 0,
      oneAccountPrice: 0,
      twoAccountPrice: 0,
      threeAccountPrice: 0,
      accountLimit: 0,
      isAdmin: false,
      password: "",
      firstName: "کاربر",
      lastName: "مهمان",
      email: "",
      mobile: "",
      phone: "",
      RemainigCapacity: 0,
      userState: true,
      avatar: undefined,
      confirmPassword: null,
      cunsumedTraffic: 0,
    },

    FirstNameRules: [(v) => !!v || "لطفا نام را وارد نمایید"],
    LastNameRules: [(v) => !!v || "لطفا نام خانوادگی را وارد نمایید"],

    AvatarRules: [
      (value) =>
        !value || value.size < 600000 || "سایز مجاز 600 کیلوبایت میباشد",
    ],
    // ConfirmPasswordRules: [(v) => !!v || "لطفا تکرار رمز عبور را وارد نمایید"],
    MobileRules: [
      (v) => !!v || "لطفا شماره موبایل را وارد نمایید",
      (v) => /^(09|9)+([0-9]){9}$/.test(v) || "شماره موبایل اشتباه است",
    ],
  }),
  computed: {
    logoUrl() {
      return this.logoName;
    },
    allRoles() {
      return this.$store.state.roles;
    },
  },
  watch: {
    dialog: {
      handler() {
        if (!this.dialog) {
          this.clearData();
        } else {
          this.getServers();
        }
        if (this.userId) this.getUser(this.userId);
      },
      deep: true,
    },
  },
  methods: {
    async getServers() {
      await request.get(`/ApiUrl/ApiUrls`).then((response) => {
        this.servers = response.data.result.result;
      });
    },
    async getUser(id) {
      await request.get(`/user/${id}`).then((response) => {
        var data = response.data.result;

        this.user.firstName = data.firstName;
        this.user.lastName = data.lastName;
        this.user.mobile = data.mobile;
        this.user.serverId = data.serverId;
        this.user.email = data.email;
        this.user.oneAccountPrice = data.oneAccountPrice;
        this.user.twoAccountPrice = data.twoAccountPrice;
        this.user.threeAccountPrice = data.threeAccountPrice;
        this.user.accountLimit = data.accountLimit;
        this.user.enable = data.enable;
        this.user.phone = data.phone;
        this.user.userState = data.userState;
        this.user.avatar = "";
        this.user.isAdmin = data.isAdmin;
        this.imageUrl = data.avatar;
        this.user.initCapacity = data.initCapacity;
        this.user.cunsumedTraffic = data.cunsumedTraffic;
      });
    },
    clickImg() {
      document.getElementById("fileInput").click();
    },
    removePreview() {
      this.imageUrl = "";
    },
    createImage(file) {
      const reader = new FileReader();

      reader.onload = (e) => {
        this.imageUrl = e.target.result;
      };
      reader.readAsDataURL(file);
    },
    onFileChange(file) {
      if (!file) {
        return;
      }
      this.hasImage = true;
      this.imageUrl = file.dataUrl;
      let output = this.dataUrl(file.dataUrl, file.info.name);
      this.user.avatar = this.dataUrl(file.dataUrl, file.info.name);
    },

    dataUrl(dataurl, filename) {
      var arr = dataurl.split(","),
        mime = arr[0].match(/:(.*?);/)[1],
        bstr = atob(arr[1]),
        n = bstr.length,
        u8arr = new Uint8Array(n);

      while (n--) {
        u8arr[n] = bstr.charCodeAt(n);
      }

      return new File([u8arr], filename, { type: mime });
    },

    getRoles() {
      this.$store.commit("getRoles");
    },

    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;

      var form_data = new FormData();

      for (var key in this.user) {
        if (this.user[key] !== "" && this.user[key] !== null)
          form_data.append(key, this.user[key]);
      }
      request.defaults.headers.common.accept = "multipart/form-data";
      if (this.userId) {
        request
          .put(`/user/${this.userId}`, form_data)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadUsers");
            this.clearData();
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      } else {
        request
          .post("/user", form_data)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadUsers");
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      }
    },

    clearData() {
      // (this.user.confirmPassword = ""),
      //   (this.user.password = ""),
      (this.user.firstName = "کاربر"),
        (this.user.lastName = "مهمان"),
        (this.user.email = ""),
        // (this.user.phone = ""),
        (this.user.selectedServers = []),
        (this.user.mobile = ""),
        (this.user.accountLimit = 0),
        (this.user.isAdmin = false),
        (this.user.avatar = undefined),
        (this.selectedRoles = []),
        (this.userRoles = []),
        (this.user.userState = true);
      this.userId = null;
    },
  },
});
</script>

<style scoped>
.v-card--reveal {
  align-items: center;
  bottom: 0;
  justify-content: center;
  opacity: 0.5;
  position: absolute;
  width: 100%;
}

.card-form-img {
  padding: 0px !important;
}

.icon-btn-modal {
  position: absolute;
  font-size: 18px !important;
  color: #fff !important;
  padding: 8px;
  border-radius: 50%;
}

.icon-btn-modal:hover {
  cursor: pointer;
}

.icon-btn-upload {
  position: absolute;
  left: 60%;
  bottom: 33%;
  color: #fff !important;
  /*padding: 8px;*/
  border-radius: 50%;
  /*background: #35495E !important;*/
  text-align: center;
  display: flex;
  margin: auto;
  justify-content: center;
  align-items: center;
  /*height: 20px !important;*/
  /*width: 20px !important;*/
}

.v-icon {
  color: #fff !important;
  font-size: 18px !important;
  text-align: center;
  background: #35495e !important;
}

.logo-title {
  text-align: center;
  display: flex;
  justify-content: center;
  margin-bottom: 15px;
}
</style>
