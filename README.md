# WebMatTwitchBot
Um simples bot para automatizar o chat do seu canal na twitch.

## Considerações:
  A finalidade deste bot é trazer uma maior proximidade entre o público da twitch e a linguagem de programação C#<br/>
  O bot, por padrão, é o mesmo usuário do canal que se deseja o bot.<br/>
  Existem dois modelos de Text to Speech (TTS) (texto para fala):<br/>
    A - Comando !Speak chama o power shell e utiliza o idioma padrão do seu windows para reproduzir a fala.<br/>
    B - (removido do código princial) - Comando !SpeakPt ou !SpeakEn chama o azure serviços cognitivos para reproduzir a fala (necessita configuração)<br/>
    C - Comando !SpeakPt ou !SpeakEn chama o google cloud api text-to-speech para reproduzir a fala (necessita configuração)<br/>

## Primeiro run
  Por segurança, o arquivo Parameters.cs vem com seus campos em branco. É necessário fazer o preenchimento antes de rodar o bot.
  
  1 - Para receber o valor da propriedade oauth, acesse https://twitchapps.com/tmi/ e conecte-se utilizando a conta que deseja ter o botchat.<br/>
  2 - Copie o codigo mostrado no item anterior e cole no campo oauth, no campo user preencha com o mesmo usuario utilizado anteriormente.<br/>
  B - (removido do código princial) - Caso você deseja utilizar o !speakPt !speakEn entre outros (azure serviços cognitivos) é necessário ir em : www.portal.azure.com, cadastrar-se, e instalar Serviço Cognitivo.<br/>
  C - Caso você deseja utilizar o !speakPt !speakEn (google cloud api text-to-speech) é necessário ir em : https://googleapis.dev/dotnet/Google.Cloud.TextToSpeech.V1/2.0.0/index.html, cadastrar-se, e instalar siga os passos recomendados pelo próprio google.<br/>

## Customização
  Você pode customizar os comandos e respostas do bot indo até Commands.cs adicionando um novo item no campo List. (Se você sentir alguma dificuldade consulte itens já inseridos)<br/>
  Você pode customizar os registros em cache indo até Cache.cs. (Vide exemplo entre Cache.cs e Commands.cs)<br/>
  Por padrão o Text-to-Speech vem desabilitado, para habilita-lo digite no console !setspeaker true
  
## Considerações finais
  O processo de criação do bot foi todo produzido em live no canal www.twitch.tv/webmat1. Qualquer dúvida, sugestão e/ou reclamação, pode ser encaminhado pela twitch também.<br/>
  Agradaço a todos que participaram do processo de criação e tiveram compreensão da não complexidade deste bot, pois a finalidade maior é torná-lo um caminho acessivel ao C#.<br/>
  
  
  
Just a bot to respond your twitch chat.

## Firstly
  The main idea is turning people on twitch next to programmming language c#.<br/>
  By default, our bot uses the same userbot and channel target.<br/>
  There are two ways to use Text-to-Speech (TTS):<br/>
    A - Command !Speak, this uses the power shell to call all librarys. So its works fine on windows<br/>
    B - (depreciated) - Command !SpeakPt or !SpeakEn, this uses azure cognitive services to do a machine speak<br/>
    C - Command !SpeakPt or !SpeakEn (etc.), this uses google cloud api text-to-speech to do a machine speak<br/>

## First run
  For security, Parameters.cs file has your fields empty. Its necessary fill it before first run.

  1 - To get the value to fill oauth field, access https://twitchapps.com/tmi/ and click on Connect button.<br/>
  2 - Copy the code on before step and fill oauth field. user field must be filled with the same user used before.<br/>
  B - (depreciated) - If you are going to use !SpeakPt or !SpeakEn (google cloud api text-to-speech) you need follow all steps in https://googleapis.dev/dotnet/Google.Cloud.TextToSpeech.V1/2.0.0/index.html; Maybe you should sign-in and install the google cloud api.<br/>

## Customization
  You can change all commands in Commands.cs, List field. (You can see examples there)<br/>
  You can change all cached items in Chache.cs. (Its good to insert a command in Commands.cs as well)<br/>
  By default TTS is disabled, to enable you must type "!setspeaker true" on console.<br/>

## Final Considerations
  This bot was built on stream in www.twitch.tv/webmat1. Feel free to send your suggestion there.<br/>
  Thank's to all people no stream who gave me feedback, suggestions and fixes.<br/>
