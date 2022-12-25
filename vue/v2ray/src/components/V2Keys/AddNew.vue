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

              <v-col class="d-flex" cols="12" sm="6">
                <v-select
                  v-model="v2Key.serverId"
                  :items="servers"
                  item-value="id"
                  item-text="title"
                  label="ُسرور"
                  solo
                ></v-select>
              </v-col>
              <v-col cols="6" sm="12" md="6">
                  <v-text-field
                    v-model="v2Key.count"
                    label="تعداد *"
                    placeholder=" "
                    autocomplete="false"
                    required
                  ></v-text-field>
                </v-col>
              <v-row>
                <v-col cols="6" sm="12" md="6">
                  <v-text-field
                    v-model="v2Key.capacity"
                    label="ترافیک *"
                    placeholder=" "
                    autocomplete="false"
                    required
                  ></v-text-field>
                </v-col>

                <v-col cols="6" sm="12" md="6">
                  <label>تاریخ :</label>
                  <date-picker  v-model="v2Key.expireDate" simple />

                </v-col>
              </v-row>
              <v-col cols="4">
                  <v-switch
                    v-model="v2Key.state"
                    :label="`وضعیت: ${
                      v2Key.state ? 'فعال' : 'غیر فعال'
                    }`"
                  ></v-switch>
                </v-col>
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
  name: "AddNewV2Key",

  data: () => ({
    id: null,
    dialog: false,
    dialogLogo: false,
    servers: [],
    valid: true,
    loading: false,
    v2Key: {
      serverId :0,
      capacity: "",
      expireDate : null,
      state: true,
      count: 1,
    },
  }),
  watch: {
    dialog: {
      handler() {
        if (!this.dialog) {
          this.clearData();
        }else{
          this.getServers();
        }
        if (this.id) 
        this.getV2Key(this.id);
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
    async getV2Key(id) {
      await request.get(`/v2Key/${id}`).then((response) => {
        var data = response.data.result;
        this.v2Key.id = id;
        this.v2Key.serverId = data.serverId;
        this.v2Key.capacity = data.capacity;
        this.v2Key.expireDate = data.expireDate;
        this.v2Key.state = data.state;
      });
    },
  
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;
      
      if (this.id) {
        request
          .put(`/v2Key/${this.id}`,this.v2Key)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadV2Keys");
            this.clearData();
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      } else {
        request
          .post("/v2Key", this.v2Key)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadV2Keys");
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      }
    },

    clearData() {
      this.selectedComplexId = null;

        (this.v2Key.serverId = 0),
        (this.v2Key.capacity = 40),
        (this.v2Key.expireDate = ""),
        (this.v2Key.count =1),
        (this.v2Key.state = true)
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
