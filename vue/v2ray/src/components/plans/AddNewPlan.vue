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
        <v-btn color="primary" dark v-bind="attrs" v-on="on">ثبت پلن</v-btn>
      </template>
      <v-card>
        <v-card-title>
          <span class="text-h5">ثبت پلن جدید</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container>
              <v-row>
                <v-col cols="6" sm="12" md="6">
                  <v-text-field
                    v-model="plan.title"
                    label="عنوان *"
                    placeholder=" "
                    autocomplete="false"
                    :rules="TitleRules"
                    required
                  ></v-text-field>
                </v-col>

                <v-col cols="6" sm="12" md="6">
                  <v-text-field
                    autocomplete="false"
                    v-model="plan.description"
                    label="توضیحات"
                    required
                  ></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    autocomplete="false"
                    v-model="plan.price"
                    label="قیمت *"
                    placeholder=" "
                    :rules="PriceRules"
                  ></v-text-field>
                </v-col>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    autocomplete="false"
                    v-model="plan.trafficCapacity"
                    label="ترافیک *"
                    placeholder=" "
                  ></v-text-field>
                </v-col>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    autocomplete="false"
                    v-model="plan.period"
                    label="مدت *"
                    placeholder=" "
                  ></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="4" sm="12" md="4">
                  <v-col>
                    <v-btn
                      large
                      color="blue-grey"
                      class="ma-2 white--text"
                      @click="clickImg"
                    >
                      بارگذاری تصویر
                    </v-btn>

                    <image-uploader
                      hidden
                      v-model="plan.image"
                      :debug="1"
                      :max-width="512"
                      :quality="0.7"
                      :auto-rotate="true"
                      :preview="false"
                      output-format="verbose"
                      :class-name="[
                        'fileinput',
                        { 'fileinput--loaded': hasImage },
                      ]"
                      :capture="false"
                      accept="image/*"
                      do-not-resize="['gif', 'svg']"
                      @input="onFileChange"
                    ></image-uploader>
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
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="4">
                  <v-switch
                    v-model="plan.planState"
                    :label="`وضعیت: ${plan.planState ? 'فعال' : 'غیر فعال'}`"
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
import ImageUploader from "vue-image-upload-resize";

import qs from "qs";
export default Vue.extend({
  name: "AddNewPlan",
  components: {
    ImageUploader,
  },
  data: () => ({
    id: null,
    hasImage: false,
    dialog: false,
    dialogLogo: false,
    valid: true,
    loading: false,
    imageUrl: "",
    plan: {
      title: "",
      description: "",
      price: 10000,
      period: 30,
      planState: true,
      image: undefined,
      trafficCapacity: 1,
    },

    TitleRules: [(v) => !!v || "لطفا عنوان را وارد نمایید"],
    PriceRules: [(v) => !!v || "لطفا قیمت را وارد نمایید"],

    ImageRules: [
      (value) =>
        !value || value.size < 600000 || "سایز مجاز 600 کیلوبایت میباشد",
    ],
  }),
  watch: {
    dialog: {
      handler() {
        if (!this.dialog) {
          this.clearData();
        }
        if (this.id) this.getPlan(this.id);
      },
      deep: true,
    },
  },
  methods: {
    async getPlan(id) {
      await request.get(`/plan/${id}`).then((response) => {
        const data = response.data.result;
        this.plan.id = id;
        this.plan.title = data.title;
        this.plan.description = data.description;
        this.plan.price = data.price;
        this.imageUrl = data.image;
        this.plan.period = data.period;
        this.plan.trafficCapacity = data.trafficCapacity;
        this.plan.planState = data.planState;
        this.plan.image = "";
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
      const output = this.dataUrl(file.dataUrl, file.info.name);
      this.plan.image = this.dataUrl(file.dataUrl, file.info.name);
      file = "";
    },

    dataUrl(dataurl, filename) {
      let arr = dataurl.split(","),
        mime = arr[0].match(/:(.*?);/)[1],
        bstr = atob(arr[1]),
        n = bstr.length,
        u8arr = new Uint8Array(n);

      while (n--) {
        u8arr[n] = bstr.charCodeAt(n);
      }

      return new File([u8arr], filename, { type: mime });
    },

    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;

      const form_data = new FormData();

      for (const key in this.plan) {
        if (this.plan[key] !== "" && this.plan[key] !== null)
          form_data.append(key, this.plan[key]);
      }

      request.defaults.headers.common.accept = "multipart/form-data";
      if (this.id) {
        request
          .put(`/plan/${this.id}`, form_data)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadPlans");
            this.clearData();
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      } else {
        request
          .post("/plan", form_data)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadPlans");
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      }
    },

    clearData() {
      // (this.plan.confirmPassword = ""),
      //   (this.plan.password = ""),
      (this.plan.title = ""),
        (this.plan.description = ""),
        (this.plan.price = 10000),
        (this.plan.period = 30),
        (this.plan.trafficCapacity = 1),
        (this.plan.planState = true),
        (this.plan.image = undefined),
        (this.id = null);
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
