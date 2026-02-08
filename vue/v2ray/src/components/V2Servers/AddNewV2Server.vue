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
      <template #activator="{ on, attrs }">
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
                <div>
                  <v-card-text class="h3"> کاربر </v-card-text>
                  <v-select
                    v-if="this.$store.state.userDetails.isAdmin"
                    v-model="v2Server.userId"
                    :items="users"
                    item-value="id"
                    item-text="fullName"
                    label="سرور"
                    solo
                  ></v-select>
                </div>
              </v-row>
              <v-row>
                <v-col cols="3" sm="12" md="3">
                  <v-text-field
                    v-model="v2Server.title"
                    label="عنوان *"
                    placeholder=" "
                    autocomplete="false"
                    required
                  ></v-text-field>
                </v-col>

                <v-col cols="3" sm="12" md="3">
                  <v-text-field
                    autocomplete="false"
                    v-model="v2Server.url"
                    label="آدرس URL"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="3" sm="12" md="3">
                  <v-text-field
                    autocomplete="false"
                    v-model="v2Server.capacity"
                    label="ظرفیت"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="3" sm="12" md="3">
                  <v-text-field
                    autocomplete="false"
                    v-model="v2Server.ip"
                    label="آدرس IP"
                    required
                  ></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="3" sm="12" md="4">
                  <v-text-field
                    autocomplete="false"
                    v-model="v2Server.userName"
                    label="نام کاربری *"
                    placeholder=" "
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="3" sm="12" md="4">
                  <v-text-field
                    autocomplete="false"
                    v-model="v2Server.password"
                    label="کلمه عبور *"
                    placeholder=" "
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="3" sm="12" md="4">
                  <v-text-field
                    autocomplete="false"
                    v-model="v2Server.port"
                    label="پورت  *"
                    placeholder=" "
                    required
                  ></v-text-field>
                </v-col>

                <v-col cols="3" sm="12" md="4">
                  <v-text-field
                    autocomplete="false"
                    v-model="v2Server.token"
                    label="توکن  "
                    placeholder=" "
                    required
                  ></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="4" v-if="this.$store.state.userDetails.isAdmin">
                  <v-switch
                    v-model="v2Server.enable"
                    :label="`فعال : ${v2Server.enable ? 'فعال' : 'غیر فعال'}`"
                  ></v-switch>
                </v-col>

                <v-col cols="4">
                  <v-switch
                    v-model="v2Server.isActive"
                    :label="`وضعیت: ${v2Server.isActive ? 'فعال' : 'غیر فعال'}`"
                  ></v-switch>
                </v-col>
                <v-col cols="4">
                  <v-switch
                    v-model="v2Server.hasLoadBalance"
                    :label="`لود بالانس: ${
                      v2Server.hasLoadBalance ? 'فعال' : 'غیر فعال'
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
    currentRows: 0,
    textFields: [],
    cities: [],
    dialogLogo: false,
    valid: true,
    loading: false,
    users: [],
    v2Server: {
      userId: null,
      hasLoadBalance: null,
      capacity: 50,
      isActive: false,
      title: "",
      token: "",
      url: "",
      ip: "",
      userName: "root",
      password: "!Q@W#E$R5t6y7u8i",
      port: 1027,
    },
  }),
  watch: {
    dialog: {
      handler() {
        if (!this.dialog) {
          if (this.currentRows.length == 0) {
            this.addEmpty();
          }
          this.clearData();
        } else {
          this.getCities();
        }

        if (this.id) this.getV2Server(this.id);
      },
      deep: true,
    },
  },
  mounted() {
    this.users = this.getUsers();
  },
  methods: {
    async getUsers() {
      await request
        .get("/user/users")
        .then((response) => {
          const data = response.data.result;
          this.users = data.result;
        })
        .catch((error) => {
          alert(error);
        })
        .finally(() => {
          this.loading = false;
        });
    },
    async addEmpty() {
      this.currentRows++;
      const tempObj = {};

      tempObj["label" + this.currentRows] = `آی پی ${this.currentRows}`;
      tempObj["value" + this.currentRows] = "";
      this.textFields.push(tempObj);
    },
    async add(ip) {
      this.currentRows++;
      const tempObj = {};

      tempObj["label" + this.currentRows] = `آی پی ${this.currentRows}`;
      tempObj["value" + this.currentRows] = ip ? ip : "";
      this.textFields.push(tempObj);
    },

    async remove(index) {
      this.currentRows--;
      this.textFields.splice(index, 1);
    },

    async getV2Server(id) {
      await request.get(`/v2Server/${id}`).then((response) => {
        const data = response.data.result;
        this.v2Server.id = id;
        this.v2Server.capacity = data.capacity;
        this.v2Server.title = data.title;
        this.v2Server.url = data.url;
        this.v2Server.swapped = data.swapped;
        this.v2Server.state = data.state;
        this.v2Server.token = data.token;
        this.v2Server.cityId = data.cityId;
        this.v2Server.hasLoadBalance = data.hasLoadBalance;
        this.v2Server.isActive = data.isActive;
        this.v2Server.ip = data.ip;
        this.v2Server.enable = data.enable;
        this.v2Server.userName = data.userName;
        this.v2Server.password = data.password;
        this.password = data.password;
        this.v2Server.userId = data.userId;

        data.iPs.forEach((ip) => {
          this.add(ip);
        });
      });
    },

    async getCities() {
      await request.get(`city/all-cities`).then((response) => {
        const data = response.data.result;
        this.cities = data.result;
      });
    },
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }

      this.textFields.forEach((textField, index) => {
        const re = textField["value" + (index + 1)];
        this.v2Server.iPs.push(re);
      });
      this.loading = true;

      if (this.id) {
        request
          .put(`/v2Server/${this.id}`, this.v2Server)
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
      for (let index = 0; index < this.currentRows.length; index++) {
        this.remove(index);
      }
      (this.currentRows = 0),
        (this.v2Server.title = ""),
        (this.v2Server.capacity = 50),
        (this.textFields = []),
        (this.v2Server.url = ""),
        (this.v2Server.userName = ""),
        (this.v2Server.token = ""),
        (this.v2Server.password = ""),
        (this.v2Server.ip = ""),
        (this.v2Server.hasLoadBalance = null),
        (this.v2Server.id = null),
        (this.v2Server.cityId = 0),
        (this.v2Server.state = false),
        (this.v2Server.swapped = false);
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
.text-fields-row {
  display: flex;
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
