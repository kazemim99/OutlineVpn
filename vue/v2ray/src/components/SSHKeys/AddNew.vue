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
        <v-btn color="primary" dark v-bind="attrs" v-on="on">ثبت کلید</v-btn>
      </template>
      <v-card>
        <v-card-title>
          <span class="text-h5">ثبت کلید جدید</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container>
              <v-row>
                <v-col class="d-flex" cols="6" sm="6">
                  <v-select
                    v-model="sshKey.serverId"
                    :items="servers"
                    item-value="id"
                    item-text="title"
                    label="ُسرور"
                    solo
                  ></v-select>
                </v-col>
                <v-col class="d-flex" cols="6" sm="6">
                  <v-text-field
                    v-model="sshKey.name"
                    label="نام "
                    required
                  ></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    v-model="sshKey.count"
                    label="تعداد  *"
                    placeholder=" "
                    autocomplete="false"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    v-model="sshKey.userName"
                    label="نام کاربری *"
                    placeholder=" "
                    autocomplete="false"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    v-model="sshKey.password"
                    label="رمز"
                    placeholder=" "
                    autocomplete="false"
                  ></v-text-field>
                </v-col>
                <v-col cols="6" sm="12" md="6">
                  <label>تاریخ :</label>
                  <date-picker v-model="sshKey.expireDate" simple />
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
  name: "AddNewSSHKey",
  data: () => ({
    id: null,
    dialog: false,
    dialogLogo: false,
    servers: [],
    valid: true,
    loading: false,
    sshKey: {
      name: "",
      serverId: null,
      password: null,
      userName: "",
      expireDate: null,
      count: 1,
    },
  }),
  watch: {
    dialog: {
      handler() {
        if (!this.dialog) {
          this.clearData();
        } else {
          this.getServers();
        }
        if (this.id) this.getSSHKey(this.id);
      },
      deep: true,
    },
  },

  methods: {
    async getServers() {
      await request.get(`/v2Server/all-servers`).then((response) => {
        var data = response.data.result;
        this.servers = data.result;
      });
    },
    async getSSHKey(id) {
      await request.get(`/sshKey/${id}`).then((response) => {
        var data = response.data.result;
        this.sshKey.id = id;
        this.sshKey.serverId = data.serverId;
        this.sshKey.password = data.password;
        this.sshKey.name = data.name;
        this.sshKey.userName = data.userName;
        this.sshKey.expireDate = data.expireDate;
        this.sshKey.count =1;
      });
    },

    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;

      if (this.id) {
        request
          .put(`/sshKey/${this.id}`, this.sshKey)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadSSHKeys");
            this.clearData();
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      } else {
        request
          .post("/sshKey", this.sshKey)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadSSHKeys");
            this.clearData();

            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      }
    },

    clearData() {
      this.sshKey.id = null;
      this.sshKey.serverId = null;
      this.sshKey.password = "";
      this.sshKey.userName = "";
      this.sshKey.name = "";
      this.sshKey.expireDate = "";
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
