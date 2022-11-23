module.exports = {
  devServer: {
    headers: { "Access-Control-Allow-Origin": "*" },
    proxy: "https://localhost:7087/",
  },
  outputDir: "C:/Users/Mostafa/OneDrive/Desktop/Outline/OutlineVpn/Outline.Api/wwwroot",
  transpileDependencies: ["vuetify"],
  css: {
    loaderOptions: {
      scss: {
        prependData: `@import "@/assets/sass/_variables.scss"; @import "@/assets/sass/_mixins.scss";`,
      },
    },
  },
};