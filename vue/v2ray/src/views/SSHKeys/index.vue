<template>
  <div>
    <Breadcrump :crumbs="crumbs" />
    <v-col class="d-flex" cols="6" sm="6"> </v-col>
    <v-data-table
      :headers="headers"
      :items="sshKeys"
      :loading="loading"
      :server-items-length="totalSSHKeys"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template v-slot:item.enable="{ item }">
        <v-switch
          v-model="item.enable"
          flat
          @change="enableKey(item.id)"
          :label="`${item.enable ? 'فعال' : 'غیر فعال'}`"
        ></v-switch>
      </template>

      <template v-slot:item.delete="{ item }">
        <v-icon medium class="mr-2" @click="deleteItem(item.id)"
          >mdi-delete</v-icon
        >
      </template>

      <template v-slot:item.changePassword="{ item }">
        <v-icon medium class="mr-2" @click="changePassword(item.id)"
          >mdi-key-change</v-icon
        >
      </template>

      <template v-slot:item.charge="{ item }">
        <v-icon medium class="mr-2" @click="charge(item.id)"
          >mdi-recycle</v-icon
        >
      </template>

      <template v-slot:item.edit="{ item }">
        <v-icon medium class="mr-2" @click="editItem(item)">mdi-pencil</v-icon>
      </template>

      <template v-slot:item.qrButton="{ item }">
        <v-icon
          large
          v-if="
            item.accountType == 2 ||
            item.accountType == 4 ||
            item.accountType == 6
          "
          @click="showQRCode(item)"
          >mdi-qrcode</v-icon
        >
      </template>
      <template v-slot:item.copyqr="{ item }">
        <v-row
          v-if="
            item.accountType == 2 ||
            item.accountType == 4 ||
            item.accountType == 6 ||
            item.accountType == 7
          "
        >
          <v-icon large class="mr-2" @click="copyToClipboardCode(item)">
            mdi-content-copy</v-icon
          >
        </v-row>
      </template>

      <template v-slot:item.copy="{ item }">
        <v-snackbar v-model="snackbar" :color="snackbarColor" right>{{
          snackbarMessage
        }}</v-snackbar>
        <v-row v-if="item.accountType == 1 || item.accountType == 3">
          <v-icon large class="mr-2" @click="copyToClipBoard(item, true)"
            >mdi-content-copy</v-icon
          >
        </v-row>

        <v-row v-if="item.accountType == 5">
          <v-icon large class="mr-2" @click="copyToClipBoardWire(item)"
            >mdi-content-copy</v-icon
          >
        </v-row>
      </template>

      <template v-slot:item.copy1="{ item }">
        <v-row v-if="item.accountType == 1 || item.accountType == 3">
          <v-icon large class="mr-2" @click="copyToClipBoard(item, false)">
            mdi-content-copy</v-icon
          >
        </v-row>
      </template>

      <template
        v-if="this.$store.state.userDetails.isAdmin"
        v-slot:item.name="{ item }"
      >
        {{ item.name }}
      </template>

      <template v-slot:top>
        <v-toolbar flat>
          <v-col cols="12">
            <template right>
              <v-row>
                <v-col class="d-flex" cols="3" sm="6">
                  <AddNew
                    v-can="'Member_Create'"
                    ref="addSSHKeyCom"
                    @reloadSSHKeys="getSSHKeys"
                  />
                </v-col>
                <v-col class="d-flex" cols="3" sm="6">
                  <v-switch
                    v-model="expired"
                    flat
                    @change="expireKey()"
                    :label="`${expired ? 'فعال' : 'غیر فعال'}`"
                  ></v-switch>
                </v-col>
              </v-row>
            </template>
          </v-col>
          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>

          <v-toolbar-title>لیست سرور ها</v-toolbar-title>
        </v-toolbar>
      </template>
      <template v-slot:header.userName="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template v-slot:activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <v-icon small :color="userName ? 'primary' : ''"
                >mdi-filter</v-icon
              >
            </v-btn>
          </template>
          <div style="background-color: white; width: 280px">
            <v-text-field
              v-model="userName"
              class="pa-4"
              type="text"
              label="جستجو"
            ></v-text-field>
            <v-btn
              @click="userName = ''"
              small
              text
              color="primary"
              class="ml-2 mb-2"
              >پاک کردن</v-btn
            >
          </div>
        </v-menu>
      </template>

      <template v-slot:header.password="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template v-slot:activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <v-icon small :color="password ? 'primary' : ''"
                >mdi-filter</v-icon
              >
            </v-btn>
          </template>
          <div style="background-color: white; width: 280px">
            <v-text-field
              v-model="password"
              class="pa-4"
              type="text"
              label="جستجو"
            ></v-text-field>
            <v-btn
              @click="password = ''"
              small
              text
              color="primary"
              class="ml-2 mb-2"
              >پاک کردن</v-btn
            >
          </div>
        </v-menu>
      </template>

      <template v-slot:header.codeFil="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template v-slot:activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <v-icon small :color="codeFil ? 'primary' : ''">mdi-filter</v-icon>
            </v-btn>
          </template>
          <div style="background-color: white; width: 280px">
            <v-text-field
              v-model="codeFil"
              class="pa-4"
              type="text"
              label="جستجو"
            ></v-text-field>
            <v-btn
              @click="codeFil = ''"
              small
              text
              color="primary"
              class="ml-2 mb-2"
              >پاک کردن</v-btn
            >
          </div>
        </v-menu>
      </template>
      <template v-slot:item.chargeDate="{ item }">
        <v-badge color="green" :content="item.chargeDate"> </v-badge>
      </template>

      <template v-slot:item.expireDate="{ item }">
        <v-badge color="blue" :content="item.expireDate"> </v-badge>
      </template>
    </v-data-table>

    <v-dialog v-model="modal" max-width="250">
      <v-card>
        <v-card-title>اکانت V2Ray</v-card-title>
        <v-card-text>
          <div v-if="qrData">
            <vue-qrcode-component
              :text="qrData"
              :size="200"
              error-level="L"
              @click="copyToClipboardCode"
            />
          </div>
          <div v-else>No QR Code to display</div>
        </v-card-text>
        <v-card-actions>
          <v-btn color="primary" @click="modal = false">بستن</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-pagination
      v-model="options.page"
      @input="next"
      :length="this.pages"
      :total-visible="7"
    ></v-pagination>
  </div>
</template>
<script>
import request from "@/utils/request";
import AddNew from "@/components/SSHKeys/AddNew.vue";
import Breadcrump from "@/components/common/Breadcrump.vue";
import Vue from "vue";
import VueQrcodeComponent from "vue-qrcode-component";

export default {
  name: "SSHKeys",
  components: {
    AddNew,
    Breadcrump,
    VueQrcodeComponent,
  },
  data() {
    return {
      crumbs: [
        {
          text: "خانه",
          disabled: false,
          href: "/",
        },
        {
          text: "کلیدها",
          disabled: true,
        },
      ],

      snackbar: false,
      snackbarMessage: "کپی شد",
      snackbarColor: "success",

      totalSSHKeys: 0,
      switchLoading: null,
      pages: 0,
      modal: false,
      qrData: "",
      servers: [],
      serverId: 0,
      expired: false,
      serverid: null,
      isActive: null,
      userName: null,
      password: null,
      codeFil: null,
      sshKeys: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },
      headers: [
        { text: "نام کاربری", value: "userName", sortable: true },
        { text: "رمز عبور", value: "password", sortable: false },
        { text: "کد", value: "codeFil", sortable: false, width: "1%"},
        { text: "وضعیت ترافیک", value: "trafficExpired", sortable: false },
        { text: "ترافیک کل", value: "totalTraffic", sortable: true },
        { text: "ترافیک مصرف شده", value: "usedTraffic", sortable: true },
        { text: "زمان ایجاد / تمدید", value: "chargeDate", sortable: true },
        { text: "تاریخ انقضا", value: "expireDate", sortable: true },
        { text: "نام ", value: "name", sortable: true },
        { text: "پروتکل ", value: "protocol", sortable: true },
        { text: "وضعیت", value: "enable", sortable: true },
        { text: "تمدید", value: "charge", sortable: false },
        { text: "تغییر رمز", value: "changePassword", sortable: false },
        { text: "ویرایش", value: "edit", sortable: false },
        { text: "حذف", value: "delete", sortable: false },

        { text: "", value: "qrButton", sortable: false },
        { text: "", value: "copyqr", sortable: false },
        { text: "", value: "qrButtonWireGuard", sortable: false },
        { text: "", value: "downlaodWireGuard", sortable: false },

        { text: "", value: "copy", sortable: false },
        { text: "", value: "copy1", sortable: false },
      ],
    };
  },
  computed: {
    filteredHeaders() {
      return this.headers.filter((header) => this.showHeader(header.value));
    },
  },
  watch: {
    options: {
      handler() {
        this.getSSHKeys();
      },
      deep: true,
    },

    password: function () {
      if (this.password.length > 5 || this.password.length === 0) {
        this.options.page = 1;
        this.options.password = this.password;

        if (this.password.length > 5) this.getSSHKeys();
      }
    },
    codeFil: function () {
      if (this.codeFil.length > 5 || this.codeFil.length === 0) {
        this.options.page = 1;
        this.options.codeFil = this.codeFil;

        if (this.codeFil.length > 5) this.getSSHKeys();
      }
    },
    userName: function () {
      if (this.userName.length > 2 || this.userName.length === 0)
        this.options.page = 1;
      this.options.userName = this.userName;

      if (this.userName.length > 3) this.getSSHKeys();
    },
  },
  created() {
    this.getServers();
    this.getSSHKeys();
  },

  methods: {
    showHeader(header) {
      // Logic to determine whether to show the header based on the header name
      if (header === "name") {
        return this.showName;
      } else if (header === "age") {
        return this.showAge;
      } else if (header === "gender") {
        return this.showGender;
      }
      return false; // Default case, hide if not found
    },
    async getServers() {
      // await request.get(`/v2Server/all-servers`).then((response) => {
      //   var data = response.data.result;
      //   this.servers = data.result;
      // });
    },

    showQRCode(item) {
      // Generate QR code data based on item data
      this.qrData = item.code;

      // Optionally, add more parameters like type, security, etc.
      this.modal = true;
    },
    expireKey() {
      this.options.page = 1;
      this.options.expired = this.expired;
      this.options.sortBy = "enable";
      this.getSSHKeys();
    },

    serverKeys() {
      this.options.page = 1;
      this.options.serverid = this.serverId;
      this.options.sortBy = "createdAt";
      this.getSSHKeys();
    },
    async editItem(item) {
      this.$refs.addSSHKeyCom.dialog = true;
      this.$refs.addSSHKeyCom.id = item.id;
    },

    copyToClipboardCode(item) {
      // Copy QR code data to clipboard

      navigator.clipboard.writeText(item.code);
      this.snackbar = true;
      setTimeout(() => {
        this.snackbar = false;
      }, 2000);
    },
    copyToClipBoardWire(item) {
      // Copy QR code data to clipboard

      let co = `پروفایل دانلود: https://iranv2ray.com/api/publicData/get-wireguard-config-file/${item.userName}
لینک برنامه اندروید: https://download.wireguard.com/android-client/com.wireguard.android-1.0.20231018.apk 
لینک برنامه آیفون:https://itunes.apple.com/us/app/wireguard/id1441195209?ls=1&mt=8 
تاریخ اعتبار :  ${item.expireDate} 
آموزش استفاده :https://my.uupload.ir/dl/yoOK6DRX `;

      navigator.clipboard.writeText(co);
      this.snackbar = true;
      setTimeout(() => {
        this.snackbar = false;
      }, 2000);
    },
    copyToClipBoard(item, withToturial) {
      this.snackbar = true;
      setTimeout(() => {
        this.snackbar = false;
      }, 2000);

      if (withToturial) {
        this.snackbarMessage = "با اموزش کپی شد";
      } else {
        this.snackbarMessage = "کپی شد";
      }
      let textToCopy = "";
      if (item.accountType == 1) {
        textToCopy = ` وارد لینک زیر شوید و آموزش داخل صفحه را ببنید \n ${item.code}`;
      }
      if (item.accountType == 1) {
        if (withToturial) {
          textToCopy = `username: ${item.userName} \n password: ${item.password} \n  server : ${item.server}.iransshvpn.com \n  Port : ${item.port} \nتاریخ اعتبار : ${item.expireDate} \nدانلو برنامه اندروید : https://my.uupload.ir/dl/v9pdXMWM \n دانلود برنامه آیفون : https://apps.apple.com/us/app/napsternetv/id1629465476 \n آموزش اتصال اندروید: https://my.uupload.ir/dl/v9p1WVDr \n آموزش اتصال آیفون : https://my.uupload.ir/dl/VX7QEBWR`;
        } else {
          textToCopy = `username: ${item.userName} \n password: ${item.password} \n server : ${item.server}.iransshvpn.com \n تاریخ اعتبار : ${item.expireDate} \n Port : ${item.port} \n`;
        }
      }
      if (navigator.clipboard && window.isSecureContext) {
        navigator.clipboard
          .writeText(textToCopy)
          .then(() => {
            console.log(textToCopy);
          })
          .catch(() => {
            alert("خطا در کپی");
          });
      } else {
        const textArea = document.createElement("textarea");
        textArea.value = textToCopy;

        // Move textarea out of the viewport so it's not visible
        textArea.style.position = "absolute";
        textArea.style.left = "-999999px";

        document.body.prepend(textArea);
        textArea.select();

        try {
          document.execCommand("copy");
        } catch (error) {
          console.error(error);
        } finally {
          textArea.remove();
        }
      }
    },
    async enableKey(id) {
      request.put(`/SSHKey/change-state/${id}`).then(() => {
        this.getSSHKeys();
      });
    },

    async changePassword(id) {
      Vue.swal({
        title: "آیا مطمئن  هستید",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "بله",
        cancelButtonText: "انصراف",
      }).then((result) => {
        if (result.isConfirmed) {
          request
            .post(`/sshkey/change-password/${id}`)
            .then(() => {
              Vue.swal("", "رمز با موفقیت تغییر کرد", "success");
              this.getSSHKeys();
            })
            .finally(() => {
              this.uploadLoading = false;
            });
        }
      });
    },

    async charge(id) {
      Vue.swal({
        title: "آیا مطمئن  هستید",
        icon: "warning",
        input: "select",
        inputOptions: {
          900: "3 ماه کمتر",
          600: "2 ماه کمتر",
          300: "1 ماه کمتر",
          30: "1 ماهه",
          60: "2 ماهه",
          90: "3 ماهه",
        },
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "بله ,تمدید شود",
        cancelButtonText: "انصراف",
      }).then((result) => {
        if (result.isConfirmed) {
          debugger;
          if (result.value == "900") result.value = -90;
          if (result.value == "600") result.value = -60;
          if (result.value == "300") result.value = -30;
          request
            .post(`/sshkey/charge/${id}/${result.value}`)
            .then(() => {
              Vue.swal("", "اکانت با موفقیت تمدید گردید", "success");
              this.getSSHKeys();
            })
            .finally(() => {
              this.uploadLoading = false;
            });
        }
      });
    },

    async deleteItem(id) {
      Vue.swal({
        title: "ایا مطمئن  هستید",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "بله ,حذف شود",
        cancelButtonText: "انصراف",
      }).then((result) => {
        if (result.isConfirmed) {
          request
            .delete(`/SSHKey/${id}`)
            .then(() => {
              Vue.swal("", "کلید با موفقیت حذف گردید", "success");
              this.getSSHKeys();
            })
            .finally(() => {
              this.uploadLoading = false;
            });
        }
      });
    },

    next(page) {
      this.options.page = page;
      this.SSHKeys();
    },
    handler(event) {
      this.options = event;
    },
    GetSelectedState(state) {
      this.state = state;
    },

    async getSSHKeys() {
      const { sortBy, sortDesc, page, itemsPerPage } = this.options;
      this.loading = true;

      const filterQuery = Object.keys(this.options)
        .filter(
          (x) => this.options[x] !== null && this.options[x] !== undefined
        )
        .map((key) => `${key}=${this.options[key]}`)
        .join("&");

      this.loading = true;
      await request
        .get("/sshkey/filter?" + filterQuery)
        .then((response) => {
          var data = response.data.result;
          this.sshKeys = data.result;
          this.totalSSHKeys = data.totalItems;
          this.pages = data.pageCount;
        })
        .catch((error) => {
          alert(error);
        })
        .finally(() => {
          this.loading = false;
        });
    },
  },
};
</script>
