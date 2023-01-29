<template>
  <v-container>
    <v-row justify="center">
      <v-row justify="right">
        <v-col md="12">
          <h1>آموزش استفاده :</h1>
        </v-col>
      </v-row>
      <br />
      <br />
      <v-col cols="12">
        <h3>آموزش اتصال با Netmode</h3>
        <h4>
          ابتدا برای اتصال از
          <v-btn link to="/dashboard/buy-vpn">صفحه اصلی</v-btn>
          اطلاعات اتصال را دریافت کنید
        </h4>
      </v-col>
      <br />
      <br />
      <v-col md="6" sm="12">
        <v-img src="~@/assets/images/toturial/1.jpg"></v-img>
      </v-col>
      <v-col md="6" sm="12">
        <v-img src="~@/assets/images/toturial/2.jpg"></v-img>
      </v-col>
      <v-col md="6" sm="12">
        <v-img src="~@/assets/images/toturial/3.jpg"></v-img>
      </v-col>
      <v-col md="6" sm="12">
        <v-img src="~@/assets/images/toturial/4.jpg"></v-img>
      </v-col>

      <v-col md="6" sm="12">
        <v-img src="~@/assets/images/toturial/5.jpg"></v-img>
      </v-col>
      <v-col md="6" sm="12">
        <v-img src="~@/assets/images/toturial/6.jpg"></v-img>
      </v-col>

      <v-col md="6" sm="12">
        <v-img width="70%" src="~@/assets/images/toturial/7.jpg"></v-img>
      </v-col>
    </v-row>
  </v-container>
</template>

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
