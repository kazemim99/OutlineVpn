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
        <v-btn color="primary" dark v-bind="attrs" v-on="on">ثبت DNS</v-btn>
      </template>
      <v-card>
        <v-card-title>
          <span class="text-h5">ثبت DNS جدید</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container>
              <v-row>
                <div>
                  <v-card-text class="h3"> کاربر </v-card-text>
                  <v-select
                    v-if="this.$store.state.userDetails.isAdmin"
                    v-model="dnsRecords.userId"
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
                    v-model="dnsRecords.title"
                    label="IP *"
                    placeholder=" "
                    autocomplete="false"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="4" v-if="this.$store.state.userDetails.isAdmin">
                  <v-switch
                    v-model="dnsRecords.enable"
                    :label="`فعال : ${dnsRecords.enable ? 'فعال' : 'غیر فعال'}`"
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
  name: "AddNewDNSRecord",

  data: () => ({
    id: null,
    dialog: false,
    currentRows: 0,
    dialogLogo: false,
    valid: true,
    loading: false,
  }),
  watch: {
    dialog: {
      handler() {
        if (!this.dialog) {
          if (this.currentRows.length == 0) {
            this.addEmpty();
          }
          this.clearData();
        }
        if (this.id) this.getV2Server(this.id);
      },
      deep: true,
    },
  },
  methods: {
    async getV2Server(id) {
      await request.get(`/dnsRecords/${id}`).then((response) => {
        var data = response.data.result;
      });
    },

    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }

      this.textFields.forEach((textField, index) => {
        let re = textField["value" + (index + 1)];
        this.dnsRecords.iPs.push(re);
      });
      this.loading = true;

      if (this.id) {
        request
          .put(`/dnsRecords/${this.id}`, this.dnsRecords)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloaddnsRecords");
            this.clearData();
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      } else {
        request
          .post("/dnsRecords", this.dnsRecords)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloaddnsRecords");
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      }
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
