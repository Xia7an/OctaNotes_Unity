#import "@preview/showybox:2.0.4":*

#set page(
	margin: 32pt
)

#set text(
  font: "Hiragino Kaku Gothic ProN",
	
)
#set heading(
  numbering: "1."
  )

#set enum(
	numbering: "①"
)

#set figure(numbering: "1",supplement: "図")

#set figure.caption(separator: ". ")
#show figure.where(kind: table): set figure.caption(position: top)
#show figure.where(kind: table): set figure(supplement:"表")
#show heading: set text(font: "Hiragino Kaku Gothic ProN")


#align(center)[#image("img/Logo_Black.png", width: 70%)]
#align(center)[#text(size: 20pt)[-- 遊び方 --]]

#columns(2)[
// = コントローラーの使い方
// 　OctaNotesでは、アーケードコントローラーを用います。8つのボタンと1つのレバーが付いていますが、ゲームではボタンのみを用います。
// 

= 楽曲を選ぼう！
　まずはプレイする楽曲を選択します。楽曲を選ぶ画面は、次のようになっています。
#figure()[#image("img/SongSelect.png")]
+ コントローラーのボタンと操作の対応表です。
+ 難易度選択。DUALが初心者向け、QUADが中級者向け、OCTAが上級者向けです。
+ 楽曲一覧

= オプションを設定しよう！
　次に、オプションからノーツの流れる速度を調整します
]