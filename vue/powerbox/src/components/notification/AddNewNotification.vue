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
        <v-btn color="primary" dark v-bind="attrs" v-on="on"
          >{{ notificationId ? "ویرایش" : "افزودن" }}
        </v-btn>
      </template>
      <v-card>
        <v-card-title>
          <span class="text-h5">ویرایش</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container>
              <v-row>
                <v-col cols="4" sm="12" md="4">
                  <v-select
                    v-model="notification.type"
                    :items="this.notificationType()"
                    item-value="id"
                    item-text="text"
                    outlined
                    label="نوع"
                  ></v-select>
                </v-col>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    outlined
                    clearable
                    v-model="notification.title"
                    label="عنوان"
                    placeholder=" "
                    autocomplete="false"
                    required
                  ></v-text-field>
                </v-col>
              </v-row>

              <v-row>
                <v-col cols="12" sm="12" md="12">
                  <v-textarea
                    label="متن * "
                    auto-grow
                    v-model="notification.content"
                    outlined
                    rows="2"
                    row-height="25"
                    :rules="ContnetRules"
                    shaped
                  ></v-textarea>
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
  name: "AddNewNotification",
  components: {},
  data: () => ({
    dialog: false,
    valid: false,
    notificationId: null,
    loading: false,
    notification: {
      content: null,
      type: null,
      title: null,
    },
    ContnetRules: [(v) => !!v || "لطفا متن پیام را وارد نمایید"],
  }),

  watch: {
    dialog: {
      handler() {
        if (!this.dialog) {
          (this.notification.title = ""),
            (this.notification.type = ""),
            (this.notification.content = ""),
            (this.notification.id = "");
        }
        if (this.notificationId) this.getNotification(this.notificationId);
      },
      deep: true,
    },
  },
  methods: {
    async getNotification(id) {
      await request.get(`/notification/${id}`).then((response) => {
        var data = response.data.result;
        (this.notification.title = data.title),
          (this.notification.type = data.type),
          (this.notification.content = data.content);
      });
    },
    notificationType() {
      return [
        { id: 1, text: "درب دستگاه باز مانده" },
        { id: 2, text: "لاکر ۳۰ دقیقه بلاک شد" },
        { id: 3, text: "لاکر برای همیشه بلاک شد" },
        { id: 4, text: "لاکر باز مانده است" },
        { id: 5, text: "لاکر آنبلاک شد" },
        { id: 6, text: "موبایل در لاکر جامانده است" },
        { id: 7, text: "لاکر به وسیله کد پین باز شد" },
        { id: 8, text: "اتصال برق دستگاه قطع شد" },
      ];
    },
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;

      if (this.notificationId) {
        request
          .put(`/notification/${this.notificationId}`, this.notification)
          .then(() => {
            this.dialog = false;
            this.$emit("reloadNotification");
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
