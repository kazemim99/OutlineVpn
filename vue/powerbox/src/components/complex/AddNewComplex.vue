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
        <v-btn
          color="primary"
          v-can="GetPerm()"
          dark
          v-bind="attrs"
          v-on="on"
          >{{ isUnit ? "افزودن زیر مجموعه" : "افزودن مجموعه"}}</v-btn
        >
      </template>
      <v-card>
        <v-card-title>
          <span class="text-h5">{{
            !isUnit ? "مجموعه جدید" : "زیر مجموعه جدید"
          }}</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container>
              <v-row>
                <v-col cols="6" sm="12" md="6">
                  <v-text-field
                    solo
                    clearable
                    v-model="complex.nameFa"
                    label="نام فارسی *"
                    placeholder=" "
                    autocomplete="false"
                    :rules="NameFaRules"
                    required
                  ></v-text-field>
                </v-col>

                <v-col cols="6" sm="12" md="6">
                  <v-text-field
                    solo
                    clearable
                    v-model="complex.nameEn"
                    label="نام انگلیسی *"
                    placeholder=" "
                    autocomplete="false"
                    :rules="NameEnRules"
                    required
                  ></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="12" sm="12" md="12">
                  <v-textarea
                    label="ادرس "
                    auto-grow
                    v-model="complex.address"
                    outlined
                    rows="2"
                    row-height="25"
                    :rules="AddressRules"
                    shaped
                  ></v-textarea>
                </v-col>
                <v-col cols="12" sm="12" md="12">
                  <v-textarea
                    label="توضیحات "
                    auto-grow
                    v-model="complex.desctiption"
                    outlined
                    rows="3"
                    row-height="25"
                    shaped
                  ></v-textarea>
                </v-col>
              </v-row>

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
                    :debug="1"
                    :maxWidth="512"
                    :quality="0.7"
                    :autoRotate="true"
                    :preview="false"
                    outputFormat="verbose"
                    :className="[
                      'fileinput',
                      { 'fileinput--loaded': hasImage },
                    ]"
                    :capture="false"
                    accept="image/*"
                    doNotResize="['gif', 'svg']"
                    @input="onFileChange"
                  ></image-uploader>
                </v-col>
                <small style="display: block; color: #e91e63">
                  پسوند‌های مجاز: ".png", ".jpg",".jpeg"
                </small>
                <v-col>
                  <!-- <div class="my-2">
                    <v-btn color="error"  x-small dark>
                     حذف
                    </v-btn>
                  </div> -->
                  <v-img
                    :src="imageUrl"
                    style="border: 1px dashed #ccc; max-height: 300px"
                  />
                </v-col>
                <v-row>
                  <v-col cols="4">
                    <v-switch
                      v-model="complex.state"
                      :label="`وضعیت: ${complex.state ? 'فعال' : 'غیر فعال'}`"
                    ></v-switch>
                  </v-col>
                  <!-- <v-col cols="4" v-if="!isUnit">
                    <v-switch
                      v-model="complex.hasUnits"
                      :label="
                        `زیر مجموعه: ${complex.hasUnits ? 'دارد' : 'ندارد'}`
                      "
                    ></v-switch>
                  </v-col> -->
                </v-row>
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
import ImageUploader from "vue-image-upload-resize";
export default Vue.extend({
  name: "AddNewComplex",
  components: {
    ImageUploader,
  },
  data: () => ({
    complexId: null,
    hasImage: false,
    dialog: false,
    valid: true,
    loading: false,
    imageUrl: "",
    isUnit: false,
    complex: {
      nameEn: "",
      isMain: false,
      nameFa: "",
      description: "",
      address: "",
      avatar: "",
      parentId: "",
      state: true,
      hasUnits: true,
    },

    NameFaRules: [(v) => !!v || "لطفا نام فارسی را وارد نمایید"],
    NameEnRules: [(v) => !!v || "لطفا نام انگلیسی را وارد نمایید"],
    AddressRules: [(v) => !!v || "لطفا آدرس را وارد نمایید"],
    AvatarRules: [
      (value) =>
        !value || value.size < 2000000 || "سایز مجاز 2 مگابایت  میباشد",
    ],
  }),
  created() {
    if (this.$route.params.id) {
      this.isUnit = true;
      this.complex.parentId = this.$route.params.id;
    }
  },
  computed: {},
  watch: {
    dialog: {
      handler() {
        if (!this.dialog) {
          (this.complex.nameEn = ""),
            (this.complex.nameFa = ""),
            (this.complex.description = ""),
            (this.complex.address = ""),
            (this.complex.avatar = ""),
            (this.complex.parentId = null),
            (this.complex.avatar = ""),
            (this.imageUrl = ""),
            (this.isMain = false),
            // (this.complex.hasUnits = false);
            (this.complexId = null);
        }
        if (this.complexId) this.getComplex(this.complexId);
      },
      deep: true,
    },
  },
  methods: {
    GetPerm() {
     return this.isUnit ? "Create_SubComplex":"Create_Complex";
    },
    async getComplex(id) {
      await request.get(`/complex/${id}`).then((response) => {
        var data = response.data.result;
        this.complex.nameFa = data.nameFa;
        this.complex.isMain = data.isMain;
        this.complex.nameEn = data.nameEn;
        this.complex.address = data.address;
        this.complex.parentId = data.parentId == null ? "" : data.parentId;
        this.complex.description = data.description;
        this.complex.state = data.state;
        this.complex.avatar = "";
        this.imageUrl = data.avatar;
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
      this.complex.avatar = file;
      this.complex.avatar = this.dataUrl(file.dataUrl, file.info.name);
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
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;

      var form_data = new FormData();

      for (var key in this.complex) {
        form_data.append(key, this.complex[key]);
      }
      request.defaults.headers.common.accept = "multipart/form-data";
      if (this.complexId) {
        request
          .put(`/complex/${this.complexId}`, form_data)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadComplexes");
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      } else {
        request
          .post("/complex", form_data)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadComplexes");
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
