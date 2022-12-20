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
        <v-btn color="primary" dark v-bind="attrs" v-on="on">ثبت سرور</v-btn>
      </template>
      <v-card>
        <v-card-title>
          <span class="text-h5">ثبت سرور جدید</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container>

              <v-col class="d-flex" cols="12" sm="6">
                <!-- <v-select
                  v-model="v2Server.cityId"
                  :items="cities"
                  item-value="id"
                  item-text="title"
                  label="شهر"
                  solo
                ></v-select> -->
              </v-col>

              <v-row>
                <v-col cols="6" sm="12" md="6">
                  <v-text-field
                    v-model="v2Server.title"
                    label="عنوان *"
                    placeholder=" "
                    autocomplete="false"
                    required
                  ></v-text-field>
                </v-col>

                <v-col cols="6" sm="12" md="6">
                  <v-text-field
                    autocomplete="false"
                    v-model="v2Server.url"
                    label="آدرس URL"
                    required
                  ></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    autocomplete="false"
                    v-model="v2Server.userName"
                    label="نام کاربری *"
                    placeholder=" "
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    autocomplete="false"
                    v-model="v2Server.password"
                    label="کلمه عبور *"
                    placeholder=" "
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    autocomplete="false"
                    v-model="v2Server.port"
                    label="پورت  *"
                    placeholder=" "
                    required
                  ></v-text-field>
                </v-col>
               
              </v-row>
              <v-row>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    autocomplete="false"
                    v-model="v2Server.ip"
                    label="آی پی *"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="4">
                  <v-switch
                    v-model="v2Server.isActive"
                    :label="`وضعیت: ${
                      v2Server.isActive ? 'فعال' : 'غیر فعال'
                    }`"
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
  name: "AddNewV2Server",

  data: () => ({
    id: null,
    dialog: false,
    dialogLogo: false,
    valid: true,
    loading: false,
    v2Server: {
      title: "",
      state : false,
      isActive: false,
      url: "",
      cityId: 0,
      ip: "",
      userName: "",
      password: "",
      port:4152
    },
  }),
  watch: {
    dialog: {
      handler() {
        if (!this.dialog) {
          this.clearData();
        }
        if (this.id) this.getV2Server(this.id);
      },
      deep: true,
    },
  },
 
  methods: {
    async getV2Server(id) {
      await request.get(`/v2Server/${id}`).then((response) => {
        var data = response.data.result;
        this.v2Server.id = id;
        this.v2Server.title = data.title;
        this.v2Server.url = data.url;
        this.v2Server.cityId = data.cityId;
        this.v2Server.ip = data.ip;
        this.v2Server.isActive = data.isActive;
        this.v2Server.userName = data.userName;
        this.v2Server.password = data.password;
      });
    },
  
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;
      
      if (this.id) {
        request
          .put(`/v2Server/${this.id}`,this.v2Server)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadV2Servers");
            this.clearData();
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      } else {
        request
          .post("/v2Server", this.v2Server)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadV2Servers");
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      }
    },

    clearData() {
      this.selectedComplexId = null;

        (this.v2Server.title = ""),
        (this.v2Server.url = ""),
        (this.v2Server.userName = ""),
        (this.v2Server.password = ""),
        (this.v2Server.ip = ""),
        (this.v2Server.cityId = 0),
        (this.v2Server.state = true)
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
