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
// = コントローラーの使い方
// 　OctaNotesでは、アーケードコントローラーを用います。8つのボタンと1つのレバーが付いていますが、ゲームではボタンのみを用います。
// 

= 楽曲を選ぼう！
　まずはプレイする楽曲を選択します。楽曲を選ぶ画面は、次のようになっています。
#figure()[#image("img/SongSelect.png")]
+ コントローラーのボタンと操作の対応表です。各マスの項目がコントローラー右側についている8つのボタンと対応しています。
+ 難易度選択。DUALが初心者向け、QUADが中級者向け、OCTAが上級者向けです。
+ 楽曲一覧

　左下の2つのボタンで楽曲の選択、左上の2つのボタンで難易度を選択します。

= オプションを設定しよう！
　次に、オプションからノーツの流れる速度を調整します。選曲画面で右下の「オプション」ボタンを押します。
#align(center)[#image("img/NotePreview.png")]

　左上の2つのボタンを使って「ノーツの速さ」を調整すると、ゲームをプレイする際のノーツの流れてくるスピードを調節できます。どのくらいの速さなのかは、画面右側のプレビューで確認できます。

　ちょうどいい速さになったら、「保存」ボタンを押します。


= プレイしよう！
　選曲画面でプレイする楽曲を選択して「決定ボタン」を押しましょう。

== レーンとボタンの関係
　プレイ画面では、以下のように上下4つのレーンが表示され、各レーンには①から⑧までの番号が振られています。
#align(center)[#image("img/ingame/lanelegend.png")]

　各レーンには、コントローラーのボタンが以下のように対応します。

#align(center)[#image("img/ingame/controllerlegend.png", width: 80%)]

// 　②に左手の人差し指を、③に右手の人差し指を置くようにするとプレイしやすいです。

== ノーツの種類
　OctaNotesには、 *タップ*、*ロング*、*チェイン*の3種類のノーツがあります。次のページから、各ノーツの操作方法を習得しましょう。

　ノーツには#text(fill: blue)[*青色*]、#text(fill: red)[*赤色*]、#text(fill: rgb("e6b422"))[*金色*]のものがありますが、色が異なっても、操作は同じです。

=== タップ
　タップは、以下のような見た目をしています。

#align(center)[#image("img/ingame/Tap.png")]
　このノーツが来たら、ノーツが手前の白い線と重なるタイミングで、レーンに対応するボタンを押します。

=== ロング
　ロングは、次のような見た目をしています。

#align(center)[#image("img/ingame/Long.png")]
　このノーツが来たら、最初のタップが手前の白い線と重なるタイミングで、レーンに対応するボタンを押し、ロングノーツの終わりまでボタンを押し続けます。終わりは次のような見た目をしています。

#align(center)[#image("img/ingame/LongEnd.png")]
　ロングノーツの最後は押しっぱなしてOKです。*タイミングよくボタンを離す必要はありません*。

 \ \

=== チェイン
　チェインは、次のような見た目をしており、ロングノーツの途中に出現することが多いです。
#align(center)[#image("img/ingame/Chain.png")]
　赤丸で囲まれたひし形のノーツがチェインノーツです。

　このノーツが来るタイミングで、レーンに対応するボタンを押しておきましょう。しかし、基本的にはロングノーツをきちんと押せていれば意識する必要はありません。

== 判定について
　各タップ、およびロングの最初は、叩いたタイミングによって4段階にランク付けされます。良いものから順に *PERFECT*、*GOOD*、*BAD*、*MISS*となります。

=== コンボについて
　*MISS*以外の判定でノーツを連続して叩けると、コンボが繋がります。全てのノーツを*MISS*以外の判定で叩ければ、*FULL COMBO*となります。また、全てのノーツを*PERFECT*で叩ければ *ALL PERFECT*となります。

=== 発展 : スペシャルノーツについて
　スペシャルノーツは、次のような#text(fill: rgb("e6b422"))[*金色*]のタップノーツやロングノーツです。
#align(center)[#image("img/ingame/Special.png")]
　このノーツは、叩いたタイミングが通常のノーツでは*GOOD*、*BAD*になるタイミングだったとしても、*必ずPERFECTになる*という特性を持ちます。

= 結果を確認しよう！
　楽曲が終了すると、プレイ結果が表示されます。
#align(center)[#image("img/Result.png")]

　左上に表示されるのがスコアです。

　その右下には、スコアに応じたランクが表示されます。低いほうから順に*D*, *C*, *C+*, *B*, *B+*, *A*, *A+*, *S*, *S+*, *SS*, *SS+*, *SSS*, *Θ*となります。C以上でクリアとなります。
]

