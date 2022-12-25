<template>
  <div>
    <Breadcrump :crumbs="crumbs" />

    <v-form ref="form" v-model="valid" lazy-validation>
      <v-container>
        <v-col class="d-flex" cols="6" sm="6">
          <v-select
            v-model="v2Key.fromServerId"
            :items="servers"
            item-value="id"
            item-text="title"
            label="ُسرور"
            solo
          ></v-select>
        </v-col>

        <v-col class="d-flex" cols="6" sm="6">
          <v-select
            v-model="v2Key.toServerId"
            :items="newServers"
            item-value="id"
            item-text="title"
            label="سرور جدید"
            solo
          ></v-select>
        </v-col>
      </v-container>
    </v-form>
    <v-spacer></v-spacer>
    <v-btn :loading="loading" color="blue darken-1" text @click="submit()"
      >ذخیره</v-btn
    >
  </div>
</template>

<script>
import Vue from "vue";
import request from "@/utils/request";
import Breadcrump from "@/components/common/Breadcrump.vue";

export default Vue.extend({
  name: "Swap",
  components: {
    Breadcrump,
  },
  data: () => ({
    servers: [],
    newServers: [],
    valid: true,
    loading: false,
    v2Key: {
      fromServerId: 0,
      toServerId: 0,
    },
  }),
  created() {
    this.getServers();
    this.getNewServers();
  },
  methods: {
    async getServers() {
      await request.get(`/v2Server/all-servers`).then((response) => {
        var data = response.data.result;
        this.servers = data.result;
      });
    },
    async getNewServers() {
      await request.get(`/v2Server/new-servers`).then((response) => {
        var data = response.data.result;
        this.newServers = data.result;
      });
    },
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;

      request
        .post("/v2Key/swapServerKeys", this.v2Key)
        .then(() => {
          this.loading = false;
          this.$snotify.success("کلیدها با موفقیت با انتقال داده شدند");
        })
        .catch(() => {
          this.loading = false;
        })
        .finaly(() => {
          this.loading = false;
        });
    },

    clearData() {
      (this.v2Key.serverId = 0),
        (this.v2Key.title = ""),
        (this.v2Key.count = 0);
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
