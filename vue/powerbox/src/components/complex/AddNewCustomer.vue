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
        <v-btn color="primary" dark v-bind="attrs" v-on="on">افزودن</v-btn>
      </template>
      <v-card>
        <v-card-title>
          <span class="text-h5" v-if="customerId">ویرایش </span>
          <span class="text-h5" v-else>افزودن</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container>
              <v-row>
                <v-col cols="6" sm="12" md="6">
                  <v-text-field
                    solo
                    clearable
                    v-model="customer.name"
                    label="نام"
                    placeholder=" "
                    autocomplete="false"
                    required
                  ></v-text-field>
                </v-col>

                <v-col cols="6" sm="12" md="6">
                  <v-text-field
                    solo
                    clearable
                    v-model="customer.phoneNumber"
                    label="تلفن  *"
                    placeholder=" "
                    autocomplete="false"
                    :rules="PhoneNumberRules"
                    required
                  ></v-text-field>
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
  name: "AddNewCustomer",
  components: {},
  data: () => ({
    dialog: false,
    valid: true,
    loading: false,
    customerId: null,
    imageUrl: "",
    isUnit: false,
    customer: {
      complexId: null,
      name: "",
      phoneNumber: "",
    },
    NameRules: [(v) => !!v || "لطفا نام  را وارد نمایید"],
    PhoneNumberRules: [(v) => !!v || "لطفا تلفن  را وارد نمایید"],
  }),
  computed: {},
  created() {
    this.customer.complexId = this.$route.params.id;
  },
  watch: {
    dialog: {
      handler() {
        if (!this.dialog) {
          (this.customer.nameEn = ""),
            (this.customer.name = ""),
            (this.customer.phoneNumber = ""),
            (this.customerId = null);
        }
        if (this.customerId) this.getCustomer(this.customerId);
      },
      deep: true,
    },
  },
  methods: {
    async getCustomer(id) {
      await request.get(`/customerClub/${id}`).then((response) => {
        var data = response.data.result;
        this.customer.name = data.name;
        this.customer.phoneNumber = data.phoneNumber;
      });
    },

    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;

      if (this.customerId) {
        request
          .put(`/customerClub/${this.customerId}`, this.customer)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadCustomeres");
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      } else {
        request
          .post("/customerClub", this.customer)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadCustomeres");
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
