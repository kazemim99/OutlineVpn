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
      <template v-can="'Advertises_Create'" v-slot:activator="{ on, attrs }">
        <v-btn color="primary" dark v-bind="attrs" v-on="on"
          >{{ advertiseId ? "ویرایش" : "افزودن" }}
        </v-btn>
      </template>
      <v-card>
        <v-card-title>
          <span class="text-h5">{{
            advertiseId ? "ویرایش تبلیغ" : "تبلیغ جدید"
          }}</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container>
              <v-row>
                <v-col cols="8">
                  <v-col cols="4" sm="12" md="4">
                    <v-text-field
                      outlined
                      clearable
                      v-model="advertise.name"
                      label="نام  *"
                      placeholder=" "
                      autocomplete="false"
                      :rules="nameRules"
                      required
                    ></v-text-field>
                  </v-col>

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
                  <v-col>
                    <div class="my-2">
                      <v-btn color="error" @click="removeImage" x-small dark>
                        حذف
                      </v-btn>
                    </div>
                    <v-img
                      :src="imageUrl"
                      style="
                        border: 1px dashed #ccc;
                        width: 400px;
                        heigth: 300px;
                      "
                    />
                  </v-col>
                </v-col>

                <v-divider inset vertical></v-divider>
                <v-col cols="4">
                  <v-col>
                    <v-btn
                      large
                      color="blue-grey"
                      class="ma-2 white--text"
                      @click="clickVideo"
                    >
                      بارگذاری ویدئو
                    </v-btn>
                    <v-col>
                      <video
                        width="350"
                        height="250"
                        id="video-preview"
                        controls
                        v-show="file != ''"
                        type="video/mp4"
                      />
                    </v-col>
                    <div hidden>
                      <input
                        type="file"
                        accept="video/*"
                        id="myVideo"
                        @change="handleFileUpload($event)"
                      />
                    </div>
                  </v-col>
                </v-col>
              </v-row>

              <v-row>
                <v-col cols="12" sm="12" md="3">
                  <v-text-field
                    outlined
                    type="number"
                    clearable
                    v-model="advertise.periority"
                    label="ترتیب"
                    placeholder=" "
                    autocomplete="false"
                  ></v-text-field>

                  <v-text-field
                    outlined
                    type="number"
                    clearable
                    v-model="advertise.stopImageTimeSecond"
                    label="مدت زمان توقف (ثانیه)"
                    placeholder=" "
                    autocomplete="false"
                  ></v-text-field>
                </v-col>
              </v-row>
              <v-col cols="12" sm="12" md="3">
                <v-switch
                  v-model="advertise.isFullTime"
                  :label="`مدت زمان : ${
                    advertise.isFullTime ? '24 ساعته' : ' محدود'
                  }`"
                ></v-switch>
              </v-col>
              <div v-if="!advertise.isFullTime">
                <h2>زمان شروع :</h2>
                <br />

                <v-row>
                  <v-col cols="12" sm="12" md="4">
                    <h3>شروع :</h3>
                    <v-time-picker
                      v-model="advertise.startTime"
                      elevation="15"
                      class="mt-4"
                      format="24hr"
                      scrollable
                      min="00:01"
                      max="23:59"
                    ></v-time-picker>
                  </v-col>

                  <v-col cols="12" sm="12" md="4">
                    <h3>زمان پایان :</h3>
                    <v-time-picker
                      label="sdf"
                      elevation="15"
                      v-model="advertise.endTime"
                      class="mt-4"
                      format="24hr"
                      scrollable
                      min="00:01"
                      max="23:59"
                    ></v-time-picker>
                  </v-col>
                </v-row>
              </div>
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
  name: "AddNewAdvertise",
  components: {
    ImageUploader,
  },
  data: () => ({
    imageUrl: null,
    videoUrl: null,
    hasImage: false,
    dialog: false,
    valid: false,
    advertiseId: null,
    loading: false,
    advertise: {
      isFullTime: false,
      deviceId: null,
      name: "",
      isVideo: false,
      video65: "",
      file: "",
      video: "",
      periority: 1,
      startTime: "",
      endTime: "",
      stopImageTimeSecond: 1,
    },
    nameRules: [(v) => !!v || "لطفا نام را وارد نمایید"],
  }),
  created() {
    this.advertise.deviceId = this.$route.params.id;
  },
  watch: {
    dialog: {
      handler() {
        if (!this.dialog) {
          let video = document.getElementById("video-preview");
          video.src = "";
          (this.advertise.name = ""),
            (this.imageUrl = ""),
            (this.videoUrl = ""),
            (this.advertise.file = null),
            (this.advertiseId = null),
            (this.advertise.periority = null),
            (this.advertise.startTime = ""),
            (this.advertise.endTime = ""),
            (this.advertise.stopImageTimeSecond = null);
        }
        if (this.advertiseId) this.getAdvertise(this.advertiseId);
      },
      deep: true,
    },
  },
  methods: {
    clickImg() {
      document.getElementById("fileInput").click();
    },
    clickVideo() {
      document.getElementById("myVideo").click();
    },
    async getAdvertise(id) {
      await request.get(`/advertise/${id}`).then((response) => {
        debugger;
        let video = document.getElementById("video-preview");
        var data = response.data.result;
        video.src = data.videoUrl;
        (this.advertise.name = data.name),
          (this.imageUrl = data.file),
          (this.videoUrl = data.video),
          (this.advertise.periority = data.periority),
          (this.advertise.startTime = data.startTime),
          (this.advertise.endTime = data.endTime),
          (this.advertise.stopImageTimeSecond = data.stopImageTimeSecond);
      });
    },
    onFileChange(file) {
      if (!file) {
        return;
      }
      this.advertise.isVideo = false;
      this.hasImage = true;
      this.imageUrl = file.dataUrl;
      this.advertise.file = file;
    },

    handleFileUpload(event) {
      this.advertise.isVideo = true;

      this.videoUrl = event.target.files[0];
      this.previewVideo(event.target.files[0].name);
    },

    previewVideo(fileName) {
      let video = document.getElementById("video-preview");
      let reader = new FileReader();
      reader.readAsDataURL(this.videoUrl);
      var self = this.advertise;
      reader.addEventListener("load", function () {
        video.src = reader.result;
        var arr = reader.result.split(","),
          mime = arr[0].match(/:(.*?);/)[1],
          bstr = atob(arr[1]),
          n = bstr.length,
          u8arr = new Uint8Array(n);

        while (n--) {
          u8arr[n] = bstr.charCodeAt(n);
        }

        self.file = new File([u8arr], fileName, { type: mime });
      });
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
    removeImage() {
      this.imageUrl = "";
    },
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;

      var form_data = new FormData();

      for (var key in this.advertise) {
        form_data.append(key, this.advertise[key]);
      }
      request.defaults.headers.common.accept = "multipart/form-data";

      if (this.advertiseId) {
        request
          .put(
            `/advertise/${this.advertiseId}`,
            form_data,
            { timeout: 100000 },
            {
              headers: {
                "Content-Type": "multipart/form-data",
              },
            }
          )
          .then(() => {
            this.dialog = false;
            this.$emit("reloadAdvertises");
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      } else {
        request
          .post(
            "/advertise",
            form_data,
            { timeout: 100000 },
            {
              headers: {
                "Content-Type": "multipart/form-data",
              },
            }
          )
          .then(() => {
            this.dialog = false;
            this.$emit("reloadAdvertises");
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
