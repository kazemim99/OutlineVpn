<template>
  <v-row justify="center">
    <v-card width="90%">
      <v-card-title>
        <span class="text-h5">پروفایل</span>
      </v-card-title>
      <v-card-text>
        <v-form ref="form" v-model="valid" lazy-validation>
          <v-container>
            <v-row>
              <v-col cols="6" sm="12" md="6">
                <v-text-field
                  v-model="user.firstName"
                  label="نام *"
                  placeholder=" "
                  autocomplete="false"
                  :rules="FirstNameRules"
                  required
                ></v-text-field>
              </v-col>

              <v-col cols="6" sm="12" md="6">
                <v-text-field
                  autocomplete="false"
                  v-model="user.lastName"
                  label="نام خانوادگی *"
                  :rules="LastNameRules"
                  required
                ></v-text-field>
              </v-col>
            </v-row>
            <v-row>
              <v-col cols="4" sm="12" md="4">
                <v-text-field
                  autocomplete="false"
                  v-model="user.email"
                  label="ایمیل "
                  :rules="EmailRules"
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
              <!-- <v-col cols="4" sm="12" md="4">
                <v-text-field
                  autocomplete="false"
                  v-model="user.phone"
                  label="تلفن"
                  placeholder=" "
                ></v-text-field>
              </v-col> -->
            </v-row>

            <v-row>
              <!-- <v-col cols="4" sm="12" md="4">
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
              </v-col> -->
              <!-- <v-col cols="4" sm="12" md="4">
                <v-col>
                  <v-file-input
                    v-model="user.avatar"
                    @click:clear="removePreview"
                    type="file"
                    class="input"
                    label="آواتار"
                    hint="لطفا تصویر آواتار را وارد نمایید"
                    outlined
                    dense
                    accept="image/png, image/jpeg, image/jpg"
                    truncate-length="1"
                    :rules="AvatarRules"
                    @change="onFileChange"
                  />
                </v-col>
                <small style="display: block; color: #e91e63">
                  پسوند‌های مجاز: ".png", ".jpg",".jpeg"
                </small>
                <small style="display: block; color: #e91e63">
                  حجم مجاز: 15KB
                </small>
                <v-col>
                  <v-img
                    :src="imageUrl"
                    style="border: 1px dashed #ccc; max-height: 300px"
                  />
                </v-col>
              </v-col> -->
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn :loading="loading" color="blue darken-1" text @click="submit()"
          >ذخیره</v-btn
        >
      </v-card-actions>
    </v-card>
  </v-row>
</template>

<script>
import Vue from "vue";
import request from "@/utils/request";

export default Vue.extend({
  name: "AddNewUser",

  props: ["selectedComplexId"],
  data: () => ({
    valid: true,
    loading: false,
    imageUrl: "",
    user: {
      password: "",
      complexRolesString: [],
      firstName: "",
      userState:true,
      code:"",
      isAdmin:false,
      lastName: "",
      email: null,
      mobile: "",
      phone: "",
      avatar: undefined,
      confirmPassword: "",
    },

    FirstNameRules: [(v) => !!v || "لطفا نام را وارد نمایید"],
    LastNameRules: [(v) => !!v || "لطفا نام خانوادگی را وارد نمایید"],
    EmailRules: [
      (v) => (v.length > 1 && /.+@.+/.test(v)) || "ایمیل وارد شده صحیح نیست",
    ],
    AvatarRules: [
      (value) =>
        !value || value.size < 600000 || "سایز مجاز 600 کیلوبایت میباشد",
    ],
    MobileRules: [
      (v) => !!v || "لطفا شماره موبایل را وارد نمایید",
      (v) => /^(09|9)+([0-9]){9}$/.test(v) || "شماره موبایل اشتباه است",
    ],
  }),
  computed: {
    logoUrl() {
      return this.logoName;
    },
    passwordConfirmationRule() {
      return () =>
        this.user.password === this.user.confirmPassword ||
        "تکرار رمز عبور اشتباه است";
    },
  },
  created() {
    this.getUser();
  },
  methods: {
    async getUser() {
      await request.get(`/user/profile`).then((response) => {
        var data = response.data.result;
        this.user.firstName = data.firstName;
        this.user.lastName = data.lastName;
        this.user.isAdmin = data.isAdmin;
        this.user.mobile = data.mobile;
        this.user.email = data.email;
        this.user.phone = data.phone;
        this.user.userState = data.userState;
        this.user.code = data.code;
        this.user.avatar = "";
        this.user.complexRolesString = data.ComplexRoles;
        this.imageUrl = data.avatar;
      });
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
      this.createImage(file);
    },
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;

      var form_data = new FormData();

      for (var key in this.user) {
        form_data.append(key, this.user[key]);
      }
      form_data.append("complexRolesString", JSON.stringify(this.complexRoles));
      request.defaults.headers.common.accept = "multipart/form-data";

      request
        .put("/user/update-profile", form_data)
        .then((response) => {
          alert("پروفایل با موفقیت ویرایش شد");
          // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
        })
        .finally(() => {
          this.loading = false;
        });
    },

    clearData() {
      (this.user.confirmPassword = ""),
        (this.user.password = ""),
        (this.user.isAdmin = false),
        (this.user.firstName = ""),
        (this.user.lastName = ""),
        (this.user.email = ""),
        (this.user.phone = ""),
        (this.user.mobile = ""),
        (this.user.avatar = undefined),
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
