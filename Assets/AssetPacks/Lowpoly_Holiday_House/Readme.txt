This package is compatible with The Built-in Render Pipeline by default. For compatibility with URP, please, import the "URP Assets.unitypackage", included in the project. 

In order to run demoscene with proper settings you might need to follow next steps:

- Go to Edit > Project Settings > Graphics. In the Scriptable Render Pipeline Settings field assign "URP Asset.asset"

- Go to Edit > Project Settings > Player > Other Settings > Rendering > Color Space and change the value from Gamma to Linear. 

- In newer Unity versions also check for Post Processing in  "URP Asset_Renderer.asset" if Post Processing turned off.