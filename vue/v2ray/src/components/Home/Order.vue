<template>
  <v-row justify="center">
    <v-dialog v-model="dialog" max-width="490">
      <template v-slot:activator="{ on, attrs }">
        <v-btn color="primary" dark v-bind="attrs" v-on="on"> تمدید </v-btn>
      </template>
      <v-card>
        <v-card-title class="text-h5"> </v-card-title>

        <v-card-text>
        </v-card-text>
        

        <v-form ref="form" v-model="valid" lazy-validation>
          <!-- <v-container>
              <v-col sm="12" md="12">
                <v-text-field
                  v-model="order.cardNumber"
                  label="َشماره کارت شما *"
                  placeholder=" "
                  autocomplete="false"
                  :rules="CardNumberRules"
                  required
                ></v-text-field>
              </v-col>
              <v-col sm="12" md="12">
                <v-text-field
                  v-model="order.tranactionNumber"
                  label="َشماره تراکنش *"
                  placeholder=" "
                  autocomplete="false"
                  :rules="TransactionNumberRules"
                  required
                ></v-text-field>
              </v-col>
          </v-container> -->
        </v-form>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click="dialog = false">بستن</v-btn>
          <v-btn :loading="loading" color="blue darken-1" text @click="submit()"
            >ارسال</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>



<script>
import request from "@/utils/request";

export default {
  data() {
    return {
      dialog: false,
      loading: false,
      valid: true,
      order: {},
      TransactionNumberRules: [(v) => !!v || "شماره تراکنش را وارد نمایید"],
      CardNumberRules: [
        (v) => !!v || "شماره کارت",
        (v) => (v && v.length > 15) || "شماره کارت اشتباه است",
        (v) => (v && v.length < 17) || "شماره کارت اشتباه است",
      ],
    };
  },
  methods: {
    goTo(url) {
      window.location.href = url;
    },
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;

      request
        .post("/order", this.order)
        .then((response) => {
          this.dialog = false;
          alert("اطلاعات شما ارسال و پس از تایید اکانت شما تمدید خواهد شد");
          // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
        })
        .finally(() => {
          this.loading = false;
        });
    },
  },
};
</script>


<style scoped>
.v-card {
  font-family: arial, sans-serif;
}
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
