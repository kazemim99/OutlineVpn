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
              <v-row>
                <v-col cols="6" sm="12" md="6">
                  <v-text-field
                    v-model="apiUrl.title"
                    label="عنوان *"
                    placeholder=" "
                    autocomplete="false"
                    required
                  ></v-text-field>
                </v-col>

                <v-col cols="6" sm="12" md="6">
                  <v-text-field
                    autocomplete="false"
                    v-model="apiUrl.url"
                    label="آدرس"
                    required
                  ></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    autocomplete="false"
                    v-model="apiUrl.country"
                    label="کشور *"
                    placeholder=" "
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    autocomplete="false"
                    v-model="apiUrl.ip"
                    label="آی پی *"
                    required
                  ></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="4">
                  <v-switch
                    v-model="apiUrl.state"
                    :label="`وضعیت: ${
                      apiUrl.state ? 'فعال' : 'غیر فعال'
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
  name: "AddNewApiUrl",

  data: () => ({
    id: null,
    dialog: false,
    dialogLogo: false,
    valid: true,
    loading: false,
    apiUrl: {
      title: "",
      url: "",
      country: "",
      ip: "",
      state: true,
    },
  }),
  watch: {
    dialog: {
      handler() {
        if (!this.dialog) {
          this.clearData();
        }
        if (this.id) this.getApiUrl(this.id);
      },
      deep: true,
    },
  },
  methods: {
    async getApiUrl(id) {
      await request.get(`/apiUrl/${id}`).then((response) => {
        var data = response.data.result;
        this.apiUrl.id = id;
        this.apiUrl.title = data.title;
        this.apiUrl.url = data.url;
        this.apiUrl.country = data.country;
        this.apiUrl.ip = data.ip;
        this.apiUrl.state = data.state;
      });
    },
   

    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;
      
      if (this.id) {
        request
          .put(`/apiUrl/${this.id}`,this.apiUrl)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadApiUrls");
            this.clearData();
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      } else {
        request
          .post("/apiUrl", this.apiUrl)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadApiUrls");
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      }
    },

    clearData() {
        (this.apiUrl.title = ""),
        (this.apiUrl.url = ""),
        (this.apiUrl.country = ""),
        (this.apiUrl.ip = ""),
        (this.apiUrl.state = true)
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
