#import "@preview/showybox:2.0.4":*

#set page(
	margin: 32pt
)

#set text(
  font: "Hiragino Kaku Gothic ProN",
	
)
#set heading(
  numbering: "1. "
  )

#set enum(
	numbering: "①"
)

#set figure(numbering: "1",supplement: "図")

#set figure.caption(separator: ". ")
#show figure.where(kind: table): set figure.caption(position: top)
#show figure.where(kind: table): set figure(supplement:"表")
#show heading: set text(font: "Hiragino Kaku Gothic ProN")


#align(center)[#image("img/Logo_Black.png", width: 60%)]
#align(center)[#text(size: 16pt)[-- 遊び方 --]]

#columns(2)[
= 楽曲を選ぼう！
　まずはプレイする楽曲を選びます。初回は DUAL がおすすめです。
#figure()[#image("img/SongSelect.png")]
　左下の2つのボタンで楽曲、左上の2つのボタンで難易度を選びます。

= オプションを設定しよう！
  必要なら、選曲画面の右下の「オプション」でノーツの速さを調整します。
#align(center)[#image("img/NotePreview.png")]

  見た目がちょうどよい速さになったら、「保存」ボタンを押します。


= プレイしよう！
  楽曲を選んで「決定ボタン」を押すとプレイが始まります。

== レーンとボタンの関係
  プレイ画面では、上下4つのレーンにノーツが流れてきます。
#align(center)[#image("img/ingame/lanelegend.png")]

  各レーンには対応するボタンがあります。

#align(center)[#image("img/ingame/controllerlegend.png", width: 80%)]

// 　②に左手の人差し指を、③に右手の人差し指を置くようにするとプレイしやすいです。

== ノーツの種類
  主なノーツは *タップ*、*ロング*、*チェイン* です。

  色が違っても、基本の操作は同じです。

=== タップ
  判定ラインに重なるタイミングで、対応するボタンを押します。

#align(center)[#image("img/ingame/Tap.png")]

=== ロング
  最初を押したら、終わりまで押し続けます。

#align(center)[#image("img/ingame/Long.png")]
  終わりは、ボタンを離さなくてもかまいません。

#align(center)[#image("img/ingame/LongEnd.png")]

=== チェイン
  ロングの途中に出てくる補助ノーツです。基本的には、ロングを正しく押せていれば意識しなくて大丈夫です。
#align(center)[#image("img/ingame/Chain.png")]

== 判定について
  判定は *PERFECT*、*GOOD*、*BAD*、*MISS* の4段階です。

=== コンボについて
  *MISS*以外が続くとコンボがつながります。全部を *MISS* なしで取れれば *FULL COMBO*、全部を *PERFECT* で取れれば *ALL PERFECT* です。

= 結果を確認しよう！
  楽曲が終わると、スコアとランクが表示されます。
#align(center)[#image("img/Result.png")]

  ランクは低いほうから *D*、*C*、*C+*、*B*、*B+*、*A*、*A+*、*S*、*S+*、*SS*、*SS+*、*SSS*、*Θ* です。*C* 以上でクリアです。
]

