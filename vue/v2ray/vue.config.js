module.exports = {
  devServer: {
    socket: "socket",
    headers: { "Access-Control-Allow-Origin": "*" },
    proxy: "https://localhost:7087/",
  },
  outputDir: "C:/Users/Mostafa/OutlineVpn/V2Ray.Api/wwwroot/",

  // Disable source maps for faster production builds
  productionSourceMap: false,

  // Enable parallel builds for faster compilation
  parallel: true,

  transpileDependencies: ["vuetify"],

  css: {
    // Extract CSS in production, inline in dev for faster HMR
    extract: process.env.NODE_ENV === "production",
    loaderOptions: {
      scss: {
        additionalData: `@import "@/assets/sass/_variables.scss"; @import "@/assets/sass/_mixins.scss";`,
      },
    },
  },

  chainWebpack: (config) => {
    // Disable prefetch/preload for faster initial builds
    config.plugins.delete("prefetch");
    config.plugins.delete("preload");

    // Optimize images - inline small images to reduce HTTP requests
    config.module.rule("images").set("parser", {
      dataUrlCondition: {
        maxSize: 10 * 1024, // 10kb
      },
    });
  },

  configureWebpack: {
    // Optimize chunk splitting for better caching
    optimization: {
      splitChunks: {
        chunks: "all",
        cacheGroups: {
          libs: {
            name: "chunk-libs",
            test: /[\\/]node_modules[\\/]/,
            priority: 10,
            chunks: "initial",
          },
          elementUI: {
            name: "chunk-elementUI",
            priority: 20,
            test: /[\\/]node_modules[\\/]element-ui[\\/]/,
          },
          vuetify: {
            name: "chunk-vuetify",
            priority: 20,
            test: /[\\/]node_modules[\\/]vuetify[\\/]/,
          },
        },
      },
    },
  },
};
