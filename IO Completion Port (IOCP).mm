<map version="1.0.1">
<!-- To view this file, download free mind mapping software FreeMind from http://freemind.sourceforge.net -->
<node CREATED="1421202872529" ID="ID_1548151603" MODIFIED="1421284822617" TEXT="IO Completion Port (IOCP)">
<richcontent TYPE="NOTE"><html>
  <head>
    
  </head>
  <body>
    <p>
      &#36825;&#26159;&#19968;&#31181;&#25903;&#25345;<b><font color="#006666">&#39640;&#24615;&#33021;&#65292;&#22823;&#24182;&#21457;&#30340;&#24322;&#27493;&#22788;&#29702;&#27169;&#24335;</font></b>&#65292;&#24191;&#27867;&#24212;&#29992;&#20110;&#31867;&#20284;TCP/IP&#36890;&#35759;&#24212;&#29992;&#31243;&#24207;&#20013;
    </p>
  </body>
</html>
</richcontent>
<node CREATED="1421203644977" FOLDED="true" ID="ID_861298209" LINK="http://www.codeproject.com/Articles/1052/Developing-a-Truly-Scalable-Winsock-Server-using-I" MODIFIED="1421285189475" POSITION="right" TEXT="IOCP&#x9610;&#x8ff0;&#x6587;&#x7ae0;&#x4e00;[c++&#x7248;]">
<node CREATED="1421203673141" FOLDED="true" ID="ID_916500432" MODIFIED="1421224709337" TEXT="&#x8bf4;&#x660e;&#x56fe;&#x793a;">
<node CREATED="1421221255239" ID="ID_771791791" MODIFIED="1421221262841">
<richcontent TYPE="NODE"><html>
  <head>
    
  </head>
  <body>
    <img src="img/iocp_01.jpg" />
  </body>
</html></richcontent>
</node>
</node>
<node CREATED="1421203841862" ID="ID_1778496822" LINK="http://msdn.microsoft.com/en-us/library/aa365198%28VS.85%29.aspx" MODIFIED="1421222925440" TEXT="&#x5fae;&#x8f6f;&#x5b98;&#x65b9;&#x4ecb;&#x7ecd;">
<richcontent TYPE="NOTE"><html>
  <head>
    
  </head>
  <body>
    <p>
      &#20043;&#25152;&#20197;&#39640;&#25928;&#65292;&#25105;&#35748;&#20026;&#23601;&#26159;&#19968;&#21551;&#29992;&#20102;&#32447;&#31243;&#27744;&#36991;&#20813;&#32447;&#31243;&#24320;&#38144;&#36807;&#22823;&#65292;&#24182;&#25552;&#20379;&#20102;<font color="#0000ff"><b>GetQueuedCompletionStatus</b></font>&#26041;&#27861;&#26469;<font color="#ff6600"><b>&#20197;&#39640;&#25928;&#30340;&#26041;&#24335;&#26469;&#31561;&#24453;&#24322;&#27493;IO&#36820;&#22238;</b></font>&#12290;
    </p>
    <p>
      &#32780;GetQueuedCompletionStatus&#26368;&#31934;&#22937;&#20043;&#22788;&#26159;&#37319;&#29992;&#20102;<font color="#006666">LIFO&#26041;&#24335;&#26469;&#22788;&#29702;&#38459;&#22622;&#32447;&#31243;</font>&#65292;&#26368;&#22823;&#38480;&#24230;&#30340;&#36991;&#20813;&#32447;&#31243;&#20999;&#25442;&#20174;&#32780;&#23548;&#33268;&#39640;&#25928;&#65281;&#33267;&#20110;&#24322;&#27493;
    </p>
    <p>
      IO&#26412;&#36523;&#24182;&#27809;&#24102;&#26469;&#20160;&#20040;&#20219;&#20309;&#25552;&#39640;&#65292;&#20851;&#38190;&#36824;&#26159;<font color="#006666">GetQueuedCompletionStatus&#26041;&#27861;&#20197;&#21450;&#37197;&#21512;&#35813;&#26041;&#27861;&#30340;&#31995;&#21015;&#25968;&#25454;&#32467;&#26500;&#21644;&#20854;&#20182;&#21021;&#22987;&#21270;&#26041;&#27861;</font>&#12290;
    </p>
  </body>
</html></richcontent>
<node CREATED="1421204199045" ID="ID_288929108" MODIFIED="1421204232456" TEXT="&#x7279;&#x522b;&#x9002;&#x5408;&#x5927;&#x5e76;&#x53d1;&#x7684;&#x5f02;&#x6b65;IO&#x573a;&#x666f;&#xff0c;&#x7ed3;&#x5408;&#x9884;&#x5148;&#x5206;&#x914d;&#x7684;&#x7ebf;&#x7a0b;&#x6c60;&#x5c06;&#x6781;&#x5927;&#x63d0;&#x9ad8;&#x6027;&#x80fd;"/>
<node CREATED="1421204238518" ID="ID_1171602123" MODIFIED="1421204315583" TEXT="&#x6bcf;&#x521b;&#x5efa;&#x4e00;&#x4e2a;IOCP&#xff0c;&#x7cfb;&#x7edf;&#x5185;&#x90e8;&#x4f1a;&#x521b;&#x5efa;&#x4e00;&#x4e2a;&#x961f;&#x5217;&#x6765;&#x63a5;&#x6536;&#x5f02;&#x6b65;IO&#x8bf7;&#x6c42;"/>
<node CREATED="1421204449158" ID="ID_1376361279" MODIFIED="1421204461363" TEXT="CreateIoCompletionPort ">
<node CREATED="1421204466998" ID="ID_1919523547" MODIFIED="1421213965657" TEXT="1&#x3001;&#x521b;&#x5efa;&#x4e00;&#x4e2a;IOCP(&#x5b9e;&#x9645;&#x4f1a;&#x4f34;&#x968f;&#x521b;&#x5efa;&#x4e00;&#x4e2a;&#x961f;&#x5217;)">
<node COLOR="#3300cc" CREATED="1421205123334" ID="ID_1868108537" MODIFIED="1421222001565" TEXT="&#x53ef;&#x4ee5;&#x8bbe;&#x5b9a;&#x5bf9;&#x5e94;&#x521b;&#x5efa;&#x8be5;IOCP&#x80fd;&#x652f;&#x6301;&#x7684;&#x6700;&#x5927;&#x5e76;&#x53d1;&#x5de5;&#x4f5c;&#x7ebf;&#x7a0b;&#x6570;&#xff0c;&#x4e00;&#x822c;&#x4e3a;&#xff1a;0[&#x4ee3;&#x8868;&#x5f53;&#x524d;CPU&#x6570;]&#xff0c;&#x6709;&#x65f6;&#x5efa;&#x8bae;&#x4e3a;2*CPU">
<richcontent TYPE="NOTE"><html>
  <head>
    
  </head>
  <body>
    <p>
      
    </p>
    <p>
      &#33401;&#65281;&#33401;&#65281;&#33401;&#65281;&#32416;&#27491;&#19968;&#19979;&#65292;&#36825;&#37324;CreateIoCompletionPort&#20013;&#30340;&#21442;&#25968;NumberOfConcurrentThreads&#65292;&#26159;&#35774;&#23450;&#30340;windows&#25805;&#20316;&#31995;&#32479;&#20801;&#35768;&#30340;&#24182;&#21457;&#22788;&#29702;IO Completion Packet&#30340;&#32447;&#31243;&#25968;&#65292;&#19981;&#26159;
    </p>
    <p>
      &#21019;&#24314;IOCP&#26102;&#38543;&#20043;&#20869;&#37096;&#21551;&#21160;&#30340;&#32447;&#31243;&#25968;&#65292;&#26085;&#65292;&#24443;&#24213;&#25630;&#38169;&#20102;&#65281;&#26159;<font color="#ff0000"><b>&#25509;&#21040;&#24182;&#21457;&#24322;&#27493;IO&#35831;&#27714;&#26102;&#25165;&#24314;&#31435;&#30340;&#32447;&#31243;</b></font>(&#20173;&#28982;&#19981;&#26159;&#32447;&#31243;&#27744;&#20013;&#37027;&#20010;&#24037;&#20316;&#32447;&#31243;&#65292;&#26159;&#30495;&#27491;&#22788;&#29702;IO&#30340;&#32447;&#31243;)&#65292;<font color="#ff6600">&#36825;&#37324;&#21482;&#26159;&#35774;&#23450;&#20010;&#38400;&#20540;&#32780;&#24050;&#65292;&#36798;&#21040;&#36825;&#20010;&#38400;&#20540;</font>&#65292;&#21017;&#21518;&#32493;&#30340;&#32447;&#31243;&#37117;&#24517;&#39035;&#31561;&#24453;&#65292;&#24182;&#21457;&#25968;
    </p>
    <p>
      &#23601;&#36825;&#20040;&#22823;&#65292;&#19968;&#33324;&#20026;CPU&#25968;
    </p>
    <p>
      
    </p>
    <p>
      &#22806;&#37096;&#32447;&#31243;&#26159;&#36890;&#36807;<font color="#0000ff"><b>GetQueuedCompletionStatus</b></font><font color="#000000">&#26469;&#25552;&#21462;</font><font color="#0000ff"><b>IO Completion Packet&#65292;&#22240;&#20026;&#26159;&#22810;&#32447;&#31243;&#25552;&#21462;</b></font><font color="#000000">&#65281;&#25152;&#20197;&#25552;&#20986;&#26469;&#30340;&#25968;&#25454;&#39034;&#24207;&#32943;&#23450;&#26159;&#20081;&#30340;&#65292;&#19981;&#26159;&#36981;&#20174;&#20837;&#21015;&#26102;&#30340;&#39034;&#24207;</font>
    </p>
  </body>
</html></richcontent>
</node>
</node>
<node CREATED="1421204481878" ID="ID_1294723590" MODIFIED="1421204524943" TEXT="2&#x3001;&#x5c06;&#x8be5;IOCP&#x5173;&#x8054;&#x5230;&#x4e00;&#x4e2a;&#x6216;&#x8005;&#x591a;&#x4e2a;file handle[&#x6bd4;&#x5982; socket]">
<node CREATED="1421212499302" ID="ID_824382509" MODIFIED="1421218763992" TEXT="&#x4e00;&#x65e6;&#x4e0e;IOCP&#x5173;&#x8054;&#xff0c;&#x8fd9;&#x4e2a;file handle&#x5bf9;&#x5e94;&#x7684;&#x8d44;&#x6e90;(&#x6bd4;&#x5982;&#xff1a;socket)&#x7684;status block&#x4e0d;&#x4f1a;&#x518d;&#x5237;&#x65b0;&#xff0c;&#x76f4;&#x81f3;&#x5176;&#x672c;&#x6b21;IO&#x5b8c;&#x6210;&#x4ea7;&#x751f;&#x7684;&#x6570;&#x636e;&#x5305;&#x4ece;IOCP&#x961f;&#x5217;&#x4e2d;&#x63d0;&#x51fa;&#x540e;&#x624d;&#x4f1a;&#x7ee7;&#x7eed;&#x66f4;&#x65b0;&#x8be5;file handle&#x7684;status&#xff01;"/>
<node COLOR="#cc3300" CREATED="1421216621990" ID="ID_140288203" MODIFIED="1421216669941" TEXT="&#x8fd9;&#x4e2a;&#x662f;&#x5173;&#x952e;&#xff0c;&#x4e00;&#x822c;&#x53ef;&#x4ee5;&#x5173;&#x8054;&#x6570;&#x5343;&#x751a;&#x81f3;&#x6570;&#x4e07;&#x4e2a;file handle"/>
</node>
<node CREATED="1421204539030" ID="ID_647806169" MODIFIED="1421220596481" TEXT="3&#x3001;&#x4efb;&#x4f55;&#x4e00;&#x4e2a;&#x5173;&#x8054;&#x7684;file handle&#x5982;&#x679c;&#x53d1;&#x751f;&#x4e86;IO complete&#x4e8b;&#x4ef6;&#xff0c;&#x4f1a;&#x4ea7;&#x751f;&#x4e00;IO Complete&#x6570;&#x636e;&#x5305;&#x5e76;&#x538b;&#x5165;&#x8be5;IOCP &#x5173;&#x8054;&#x7684;&#x961f;&#x5217;"/>
<node CREATED="1421218088983" ID="ID_1448599411" MODIFIED="1421222066620" TEXT="4&#x3001;&#x4e1a;&#x52a1;&#x7ebf;&#x7a0b;&#x6c60;&#x4e2d;&#x7684;&#x7ebf;&#x7a0b;Worker Thread">
<node CREATED="1421212648823" ID="ID_434333368" MODIFIED="1421219468144" TEXT="&#x8fd9;&#x4e9b;&#x4e2a;&#x7ebf;&#x7a0b;&#x662f;&#x7528;&#x6765;&#x76d1;&#x63a7;IOCP&#x7684;&#x5bf9;&#x5e94;&#x7684;&#x961f;&#x5217;&#xff01;">
<richcontent TYPE="NOTE"><html>
  <head>
    
  </head>
  <body>
    <p>
      &#35813;&#32447;&#31243;&#19978;&#36816;&#34892;<font color="#0000ff"><b>GetQueuedCompletionStatus</b></font>&#160; &#31995;&#32479;API&#26469;&#30417;&#21548;IOCP&#30340;Queue&#65292;&#19968;&#26086;&#26377;<font color="#006666">IO Completion Packet</font>&#23601;&#20250;&#34987;&#35813;API Dequeue&#20986;&#26469;<font face="Segoe UI, Lucida Grande, Verdana, Arial, Helvetica, sans-serif" color="rgb(42, 42, 42)">&#160; </font>
    </p>
    <p>
      
    </p>
    <p>
      &#21363;&#22810;&#20010;&#32447;&#31243;&#24182;&#21457;&#25552;&#21462;&#22788;&#29702;&#26469;&#33258;<font color="#006666"><b>IOCP&#38431;&#21015;</b></font>&#20013;&#30340;&#24050;&#32463;<font color="#006666"><b>&#24322;&#27493;IO</b></font><b><font color="#ff6600">&#23436;&#25104;&#21518;</font><font color="#006666">&#33719;&#21462;&#30340;&#25968;&#25454;</font></b>&#12290;
    </p>
  </body>
</html></richcontent>
</node>
<node CREATED="1421215025271" ID="ID_985912007" MODIFIED="1421219837808" TEXT="&#x4e00;&#x4e2a;&#x7ebf;&#x7a0b;&#x53ea;&#x80fd;&#x4e0e;&#x4e00;&#x4e2a;IOCP&#x5173;&#x8054;&#xff0c;&#x9664;&#x975e;&#x8be5;&#x7ebf;&#x7a0b;&#x7ed3;&#x675f;&#x9000;&#x51fa;&#xff0c;&#x4ea6;&#x6216;&#x53d6;&#x6d88;&#x5173;&#x8054;&#x5f53;&#x524d;IOCP&#xff0c;&#x4ea6;&#x6216;&#x5f53;&#x524d;IOCP&#x5173;&#x95ed;">
<richcontent TYPE="NOTE"><html>
  <head>
    
  </head>
  <body>
    <p>
      &#21482;&#35201;<font color="#006666">&#31532;&#19968;&#27425;&#35843;&#29992;</font><font color="#0000ff"><b>GetQueuedCompletionStatus</b></font>&#23601;&#23436;&#25104;&#20102;&#35813;<font color="#006666"><b>&#32447;&#31243;</b></font>&#19982;&#35813;<font color="#0000ff"><b>IOCP</b></font>&#30340;&#20851;&#32852;&#65281;&#36825;&#20010;&#19981;&#21516;&#20110;<font color="#0000ff"><b>IOCP</b></font>&#21644;<font color="#ff6600">File Handle</font>&#30340;&#20851;&#32852;&#36807;&#31243;&#21734;
    </p>
    <p>
      
    </p>
    <p>
      <font color="#0000ff">create</font>&#160;IOCP &lt;-&gt; file handle
    </p>
    <p>
      thread-&gt;<font color="#0000ff">GetQueuedCompletionStatus</font>-&gt;IOCP
    </p>
  </body>
</html></richcontent>
<node CREATED="1421219491159" ID="ID_399032021" MODIFIED="1421219507640" TEXT="&#x53cd;&#x8fc7;&#x6765;&#x5219;&#x4e00;&#x4e2a;IOCP&#x53ef;&#x5bf9;&#x5e94;&#x65e0;&#x6570;&#x4e2a;&#x7ebf;&#x7a0b;"/>
<node CREATED="1421219509478" ID="ID_711561892" MODIFIED="1421219528368" TEXT="&#x4e00;&#x4e2a;IOCP&#x53ea;&#x80fd;&#x7528;&#x4e8e;&#x4e00;&#x4e2a;&#x8fdb;&#x7a0b;&#xff0c;&#x4e0d;&#x53ef;&#x8fdb;&#x7a0b;&#x95f4;&#x5171;&#x4eab;"/>
</node>
<node CREATED="1421214395063" ID="ID_376511383" MODIFIED="1421219603615" TEXT="&#x5173;&#x4e8e;&#x963b;&#x585e;&#x5728;IOCP&#x8fd9;&#x4e9b;&#x4e2a;&#x7ebf;&#x7a0b;&#x7684;&#x5904;&#x7406;&#x987a;&#x5e8f;">
<richcontent TYPE="NOTE"><html>
  <head>
    
  </head>
  <body>
    <p>
      <font color="#006666">&#38459;&#22622;&#30340;&#19994;&#21153;&#32447;&#31243;&#36981;&#24490;LIFO&#65292;&#21363;&#22534;&#26632;&#27169;&#24335;<b>&#21518;&#20837;&#20808;&#20986;</b>&#65292;&#21363;&#26576;&#20010;&#32447;&#31243;<b>&#26368;&#21518;&#38459;&#22622;</b>&#21017;&#31995;&#32479;&#26368;&#20808;&#37322;&#25918;</font><font color="#ff6600">(&#36825;&#26679;&#20943;&#23569;&#32447;&#31243;&#20999;&#25442;&#65292;&#25552;&#39640;&#25928;&#29575;&#65281;)</font>&#65292;&#20854;&#33719;&#21462;&#30340;<font color="#0000ff">IO Completion Packet&#21017;&#26159;&#36981;&#24490;<b>&#20808;&#20837;&#20808;&#20986;</b></font>&#12290;
    </p>
    <p>
      
    </p>
    <p>
      &#25152;&#20197;&#65292;<font color="#006666"><b>&#26368;&#21518;&#38459;&#22622;&#30340;&#19994;&#21153;&#32447;&#31243;&#33719;&#21462;&#30340;Packet&#26159;&#26368;&#26089;&#23436;&#25104;&#24182;&#20837;&#21015;&#30340;&#25968;&#25454;&#21253;</b></font>&#65281;
    </p>
  </body>
</html></richcontent>
</node>
<node CREATED="1421222068438" FOLDED="true" ID="ID_1454376760" MODIFIED="1421224738377" TEXT="GetQueuedCompletionStatus">
<node CREATED="1421222120774" ID="ID_1360895410" MODIFIED="1421222141466" TEXT="&#x7ebf;&#x7a0b;&#x8c03;&#x7528;&#x8be5;&#x65b9;&#x6cd5;&#x5373;&#x8fdb;&#x5165;&#x7b49;&#x5f85;IO Completion Packet&#x72b6;&#x6001;"/>
<node CREATED="1421222145719" ID="ID_1890390659" MODIFIED="1421222297970" TEXT="&#x5982;&#x679c;queue&#x4e2d;&#x6709;Packet,&#x7acb;&#x5373;&#x63d0;&#x51fa;&#x5904;&#x7406;!&#x5904;&#x7406;&#x5b8c;&#x540e;&#x4f1a;&#x7acb;&#x5373;&#x518d;&#x6b21;&#x8c03;&#x7528;GetQueuedCompletionStatus&#x5904;&#x7406;"/>
</node>
</node>
</node>
</node>
</node>
<node CREATED="1421222934343" ID="ID_1505291269" MODIFIED="1421285215994" POSITION="right" TEXT="C# SocketAsyncEventArgs">
<richcontent TYPE="NOTE"><html>
  <head>
    
  </head>
  <body>
    <p>
      &#22522;&#20110;IOCP&#23454;&#29616;&#65292;&#20134;&#21363;&#20869;&#37096;&#24212;&#35813;&#37319;&#29992;&#20102;IOCP&#30340;&#26694;&#26550;&#65292;&#21512;&#29702;&#30340;&#20351;&#29992;&#20102;work t thread&#32447;&#31243;&#27744;&#21644;&#21512;&#29702;&#30340;&#35774;&#32622;&#20102;Concurrent IO &#32447;&#31243;
    </p>
    <p>
      &#20197;&#21450;&#22914;&#20309;&#32465;&#23450;IOCP&#25105;&#20204;&#37117;&#26080;&#38656;&#20851;&#24515;&#65292;&#21482;&#38656;&#35201;&#20351;&#29992;&#35813;&#31867;&#26469;&#22788;&#29702;TCP&#65292;&#37027;&#23601;&#20195;&#34920;&#39640;&#25928;&#65281;
    </p>
  </body>
</html></richcontent>
<node CREATED="1421285165055" ID="ID_1574155174" LINK="http://www.codeproject.com/Articles/83102/C-SocketAsyncEventArgs-High-Performance-Socket-Cod" MODIFIED="1421285196371" TEXT="codeproject &#x793a;&#x4f8b;&#x6587;&#x7ae0;">
<node CREATED="1421304342913" FOLDED="true" ID="ID_663482712" MODIFIED="1421308174236" TEXT="msdn&#x5b98;&#x65b9;&#x793a;&#x4f8b;">
<node CREATED="1421286013056" ID="ID_1131225734" LINK="https://gist.github.com/herotony/7b855fc07239d77fc8a1/#file-saea_msdn_sample_buffermanager-cs" MODIFIED="1421286322732" TEXT="BufferManager">
<richcontent TYPE="NOTE"><html>
  <head>
    
  </head>
  <body>
    <p>
      &#30001;&#20110;SAEA&#30340;<font color="#0000ff">buffer&#21463;&#25511;&#20110;<b>&#25805;&#20316;&#31995;&#32479;</b>&#32780;&#38750;<b>.net</b></font>&#65292;&#20854;&#26159;&#34987;&#25805;&#20316;&#31995;&#32479;<font color="#0000ff">pin&#20303;</font>&#30340;&#19981;&#33021;&#34987;.net&#22238;&#25910;&#65292;&#20026;&#27492;&#24403;SAEA&#36827;&#34892;&#21508;&#31181;&#20869;&#23384;&#25805;&#20316;&#26102;&#24456;&#23481;&#26131;&#23548;&#33268;<font color="#0000ff">&#20869;&#23384;&#30862;&#29255;</font>&#65281;
    </p>
    <p>
      &#25152;&#20197;&#25552;&#20379;&#35813;&#31867;&#26469;<font color="#006666">&#20998;&#37197;&#19968;&#20010;</font><font color="#0000ff">&#22823;&#22359;</font><b><font color="#006666">&#38598;&#20013;&#65292;&#36830;&#32493;</font></b><font color="#006666">&#30340;&#20869;&#23384;&#22359;&#20379;&#31243;&#24207;&#20013;&#21508;&#20010;SEAE<b>&#21453;&#22797;&#21033;&#29992;</b></font>&#26469;&#36991;&#20813;&#20869;&#23384;&#30862;&#29255;&#38382;&#39064;&#12290;
    </p>
  </body>
</html></richcontent>
<node CREATED="1421287066128" ID="ID_563200723" MODIFIED="1421287109924" TEXT="totalBytes&#xff1a;&#x5927;&#x5185;&#x5b58;&#x5757;&#x603b;&#x5927;&#x5c0f;&#xff0c;&#x4e00;&#x822c;&#x7ed9;&#x4e2a;&#x51e0;&#x5341;&#x5146;&#x751a;&#x81f3;&#x767e;&#x5146;&#x90fd;&#x884c;"/>
<node CREATED="1421287103999" ID="ID_1713783300" MODIFIED="1421287155322" TEXT="bufferSize&#xff1a;&#x5b9e;&#x9645;&#x4f9b;SAEA&#x8fdb;&#x884c;&#x5f02;&#x6b65;IO&#x64cd;&#x4f5c;&#x7684;&#x5185;&#x5b58;&#x5927;&#x5c0f;&#xff0c;&#x90fd;&#x662f;&#x7b49;&#x503c;&#x7684;&#x56fa;&#x5b9a;&#x5927;&#x5c0f;&#x5185;&#x5b58;&#x533a;&#x57df;"/>
<node CREATED="1421287162400" ID="ID_1821628362" MODIFIED="1421300563555" TEXT="m_crrentIndex&#xff1a;&#x5b9e;&#x9645;&#x5206;&#x914d;&#x5185;&#x5b58;&#x7684;&#x4f9d;&#x636e;&#xff0c;&#x5373;&#x6bcf;&#x4e2a;&#x5185;&#x5b58;&#x5757;&#x7684;&#x8d77;&#x59cb;&#x70b9;&#xff1a;Offset"/>
</node>
</node>
<node CREATED="1421304356449" ID="ID_108828467" MODIFIED="1421304376339" TEXT="SocketAsyncServer">
<node CREATED="1421305116129" ID="ID_234611571" LINK="https://gist.github.com/herotony/7b855fc07239d77fc8a1/#file-socketasyncserver_program-cs" MODIFIED="1421306720888" TEXT="Program">
<richcontent TYPE="NOTE"><html>
  <head>
    
  </head>
  <body>
    <p>
      <font color="#006666">//This variable determines the number of </font>
    </p>
    <p>
      <font color="#006666">//SocketAsyncEventArg objects put in the pool of objects for <b>receive/send. </b></font>
    </p>
    <p>
      <font color="#006666">//The value of this variable also affects the <b>Semaphore</b>. </font>
    </p>
    <p>
      <font color="#006666">//This app uses a Semaphore to ensure that the max # of connections </font>
    </p>
    <p>
      <font color="#006666">//value does not get exceeded. </font>
    </p>
    <p>
      <font color="#006666">//Max # of connections to a socket can be limited by the Windows Operating System </font>
    </p>
    <p>
      <font color="#006666">//also.</font>
    </p>
    <p>
      public const Int32 maxNumberOfConnections = 3000;
    </p>
    <p>
      
    </p>
    <p>
      <font color="#006666">//You would want a buffer size larger than 25 probably, unless you know the </font>
    </p>
    <p>
      <font color="#006666">//data will almost always be less than 25. It is just 25 in our test app.</font>
    </p>
    <p>
      public const Int32 testBufferSize = 25;
    </p>
    <p>
      
    </p>
    <p>
      <font color="#006666">//This is the maximum number of asynchronous accept operations that can be </font>
    </p>
    <p>
      <font color="#006666">//posted simultaneously. This determines the size of the pool of </font>
    </p>
    <p>
      <font color="#006666">//SocketAsyncEventArgs objects that do accept operations. Note that this </font>
    </p>
    <p>
      <font color="#006666">//is NOT the same as the maximum # of connections.</font>
    </p>
    <p>
      public const Int32 maxSimultaneousAcceptOps = 10;
    </p>
    <p>
      
    </p>
    <p>
      <font color="#006666">//The size of the queue of incoming connections for the listen socket.</font>
    </p>
    <p>
      public const Int32 backlog = 100;
    </p>
    <p>
      
    </p>
    <p>
      <font color="#006666">//For the BufferManager</font>
    </p>
    <p>
      public const Int32 opsToPreAlloc = 2;&#160;&#160;&#160;<b><font color="#006666">&#160;// 1 for receive, 1 for send</font></b>
    </p>
    <p>
      
    </p>
    <p>
      <font color="#006666">//allows excess SAEA objects in pool.</font>
    </p>
    <p>
      public const Int32 excessSaeaObjectsInPool = 1;
    </p>
    <p>
      
    </p>
    <p>
      <font color="#006666">//This number <b>must be the same as the value on the client.</b>&#160;</font>
    </p>
    <p>
      <font color="#006666">//Tells what size the message prefix will be. Don't change this unless </font>
    </p>
    <p>
      <font color="#006666">//you change the code, because 4 is the length of 32 bit integer, which </font>
    </p>
    <p>
      <font color="#006666">//is what we are using as prefix.</font>
    </p>
    <p>
      public const Int32 receivePrefixLength = 4;
    </p>
    <p>
      public const Int32 sendPrefixLength = 4;
    </p>
    <p>
      
    </p>
    <p>
      
    </p>
    <p>
      <font color="#006666">//If you make this a positive value, it will simulate some delay on the </font>
    </p>
    <p>
      <font color="#006666">//receive/send SAEA object after doing a receive operation. </font>
    </p>
    <p>
      <font color="#006666">//That would be where you would do some work on the received data, </font>
    </p>
    <p>
      <font color="#006666">//before responding to the client. </font>
    </p>
    <p>
      <font color="#006666">//<b>This is in milliseconds</b>. So a value of 1000 = 1 second delay.</font>
    </p>
    <p>
      public static readonly Int32 msDelayAfterGettingMessage = -1;
    </p>
    <p>
      
    </p>
    <p>
      
    </p>
    <p>
      <font color="#006666">// To keep a record of maximum number of simultaneous connections </font>
    </p>
    <p>
      <font color="#006666">// that occur while the server is running. This can be limited by operating </font>
    </p>
    <p>
      <font color="#006666">// system and hardware. It will not be higher than the value that you set </font>
    </p>
    <p>
      <font color="#006666">// for maxNumberOfConnections.</font>
    </p>
    <p>
      public static Int32 maxSimultaneousClientsThatWereConnected = 0;
    </p>
  </body>
</html>
</richcontent>
<node CREATED="1421305760640" ID="ID_1858593748" MODIFIED="1421305771515" TEXT="invoke">
<node CREATED="1421305772721" ID="ID_1389023056" LINK="https://gist.github.com/herotony/7b855fc07239d77fc8a1/#file-socketasyncserver_socketlistenersettings-cs" MODIFIED="1421306241306" TEXT="1&#x3001;SocketListenerSettings"/>
<node CREATED="1421305810209" ID="ID_1942508174" LINK="https://gist.github.com/herotony/7b855fc07239d77fc8a1/#file-socketasyncserver_socketlistener-cs" MODIFIED="1421306566623" TEXT="2&#x3001;SocketListener">
<node CREATED="1421306685201" ID="ID_724099088" MODIFIED="1421306687986" TEXT="invoke">
<node CREATED="1421306852609" ID="ID_572130378" MODIFIED="1421306882626" TEXT="cotr : SocketListenerSettings"/>
<node CREATED="1421306828257" ID="ID_1823620143" LINK="https://gist.github.com/herotony/7b855fc07239d77fc8a1/#file-socketasyncserver_prefixhandler-cs" MODIFIED="1421308115450" TEXT="1&#x3001;PrefixHandler">
<node CREATED="1421307903201" ID="ID_1082856925" LINK="https://gist.github.com/herotony/7b855fc07239d77fc8a1/#file-socketasyncserver_dataholdingusertoken-cs" MODIFIED="1421308297426" TEXT="DataHoldingUserToken">
<node CREATED="1421308134193" ID="ID_1236673750" MODIFIED="1421308139098" TEXT="Mediator"/>
<node CREATED="1421308145905" ID="ID_783426136" MODIFIED="1421308150906" TEXT="DataHolder"/>
</node>
</node>
<node CREATED="1421306835905" ID="ID_1759856669" LINK="https://gist.github.com/herotony/7b855fc07239d77fc8a1/#file-socketasyncserver_messagehandler-cs" MODIFIED="1421307740404" TEXT="2&#x3001;MessageHandler"/>
<node CREATED="1421306900225" ID="ID_1378383608" MODIFIED="1421306906801" TEXT="3&#x3001;BufferManager"/>
<node CREATED="1421306923009" ID="ID_1001714745" MODIFIED="1421306935242" TEXT="4&#x3001;SocketAsyncEventArgsPool for Rec/Send"/>
<node CREATED="1421306936401" ID="ID_90541497" MODIFIED="1421306945307" TEXT="5&#x3001;SocketAsyncEventArgsPool for Accept"/>
<node CREATED="1421306957857" ID="ID_290140371" MODIFIED="1421306966113" TEXT="6&#x3001;Init()"/>
<node CREATED="1421306967793" ID="ID_1191584512" MODIFIED="1421309153663" TEXT="7&#x3001;StartListen()">
<richcontent TYPE="NOTE"><html>
  <head>
    
  </head>
  <body>
    <p>
      <font color="#006666">// create the socket which listens for incoming connections</font>
    </p>
    <p>
      listenSocket = new Socket(this.socketListenerSettings.LocalEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    </p>
    <p>
      
    </p>
    <p>
      <font color="#006666">//bind it to the port</font>
    </p>
    <p>
      listenSocket.Bind(this.socketListenerSettings.LocalEndPoint);
    </p>
    <p>
      
    </p>
    <p>
      <font color="#006666">// Start the listener with a backlog of however many connections. </font>
    </p>
    <p>
      <font color="#006666">//&quot;backlog&quot; means pending connections. </font>
    </p>
    <p>
      <font color="#006666">//The backlog number is the number of clients that can wait for a </font>
    </p>
    <p>
      <font color="#006666">//SocketAsyncEventArg object that will do an accept operation. </font>
    </p>
    <p>
      <font color="#006666">//The listening socket keeps the backlog as a queue. The backlog allows </font>
    </p>
    <p>
      <font color="#006666">//for a certain # of excess clients waiting to be connected. </font>
    </p>
    <p>
      <font color="#006666">//If the backlog is maxed out, then the client will receive an error when </font>
    </p>
    <p>
      <font color="#006666">//trying to connect. </font>
    </p>
    <p>
      <font color="#006666">//max # for backlog can be limited by the operating system.</font>
    </p>
    <p>
      listenSocket.Listen(this.socketListenerSettings.Backlog);
    </p>
    <p>
      
    </p>
    <p>
      <font color="#006666">// Calls the method which will post accepts on the listening socket. </font>
    </p>
    <p>
      <font color="#006666">// This call just occurs one time from this StartListen method. </font>
    </p>
    <p>
      <font color="#006666">// After that the StartAccept method will be called in a loop.</font>
    </p>
    <p>
      StartAccept();
    </p>
  </body>
</html>
</richcontent>
<node CREATED="1421309169377" ID="ID_1390738628" MODIFIED="1421309173098" TEXT="invoke ">
<node CREATED="1421309174385" ID="ID_607662166" MODIFIED="1421309493129" TEXT="StartAccept">
<richcontent TYPE="NOTE"><html>
  <head>
    
  </head>
  <body>
    <p>
      <font color="#006666">//____________________________________________________________________________ </font>
    </p>
    <p>
      <font color="#006666">// Begins an operation to accept a connection request from the client</font>
    </p>
    <p>
      internal void StartAccept()
    </p>
    <p>
      {
    </p>
    <p>
      &#160;&#160;&#160;&#160;if (Program.watchProgramFlow == true)&#160;&#160;&#160;//for testing
    </p>
    <p>
      &#160;&#160;&#160;&#160;{
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;Program.testWriter.WriteLine(&quot;StartAccept method&quot;);
    </p>
    <p>
      &#160;&#160;&#160;&#160;}
    </p>
    <p>
      &#160;&#160;&#160;&#160;SocketAsyncEventArgs acceptEventArg;
    </p>
    <p>
      
    </p>
    <p>
      &#160;&#160; <font color="#006666">&#160;//Get a SocketAsyncEventArgs object to accept the connection. </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//Get it from the pool if there is more than one in the pool. </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//We could use zero as bottom, but one is a little safer.</font>
    </p>
    <p>
      &#160;&#160;&#160;&#160;if (this.poolOfAcceptEventArgs.Count &gt; 1)
    </p>
    <p>
      &#160;&#160;&#160;&#160;{
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;try
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;{
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;acceptEventArg = this.poolOfAcceptEventArgs.Pop();
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;}
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160; <font color="#006666">&#160;//or make a new one.</font>
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;catch
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;{
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;acceptEventArg = CreateNewSaeaForAccept(poolOfAcceptEventArgs);
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;}
    </p>
    <p>
      &#160;&#160;&#160;&#160;}
    </p>
    <p>
      &#160;&#160; <font color="#006666">&#160;//or make a new one.</font>
    </p>
    <p>
      &#160;&#160;&#160;&#160;else
    </p>
    <p>
      &#160;&#160;&#160;&#160;{
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;acceptEventArg = CreateNewSaeaForAccept(poolOfAcceptEventArgs);
    </p>
    <p>
      &#160;&#160;&#160;&#160;}
    </p>
    <p>
      &#160;&#160;&#160;&#160;if (Program.watchThreads == true)&#160;&#160;&#160;//for testing
    </p>
    <p>
      &#160;&#160;&#160;&#160;{
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArg.UserToken;
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;DealWithThreadsForTesting(&quot;StartAccept()&quot;, theAcceptOpToken);
    </p>
    <p>
      &#160;&#160;&#160;&#160;}
    </p>
    <p>
      &#160;&#160;&#160;&#160;if (Program.watchProgramFlow == true)&#160;&#160;&#160;//for testing
    </p>
    <p>
      &#160;&#160;&#160;&#160;{
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArg.UserToken;
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;Program.testWriter.WriteLine(&quot;still in StartAccept, id = &quot; + theAcceptOpToken.TokenId);
    </p>
    <p>
      &#160;&#160;&#160;&#160;}
    </p>
    <p>
      &#160;&#160;&#160;<font color="#006666">&#160;//Semaphore class is used to control access to a resource or pool of </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//resources. Enter the semaphore by calling the WaitOne method, which is </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//inherited from the WaitHandle class, and release the semaphore </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//by calling the Release method. This is a mechanism to prevent exceeding </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;// the max # of connections we specified. We'll do this before </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;// doing AcceptAsync. If maxConnections value has been reached, </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//then the application will pause here until the Semaphore gets released, </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//which happens in the CloseClientSocket method.</font>
    </p>
    <p>
      &#160;&#160;&#160;<b><font color="#0000ff">&#160;this.theMaxConnectionsEnforcer.WaitOne();</font></b>
    </p>
    <p>
      
    </p>
    <p>
      &#160;&#160;&#160;&#160;<font color="#006666">//Socket.<b>AcceptAsync</b>&#160;begins asynchronous operation to accept the connection. </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//Note the listening socket will pass info to the SocketAsyncEventArgs </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//object that has the Socket that does the accept operation. </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//If you do not create a Socket object and put it in the SAEA object </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//before calling AcceptAsync and use the AcceptSocket property to get it, </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//then a new Socket object will be created for you by .NET.</font>
    </p>
    <p>
      &#160;&#160;&#160;<b><font color="#0000ff">&#160;bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);</font></b>
    </p>
    <p>
      &#160;&#160;&#160;<font color="#006666">&#160;//Socket.AcceptAsync returns true if the I/O operation is pending, i.e. is </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//working asynchronously. The </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//SocketAsyncEventArgs.Completed event on the acceptEventArg parameter </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//will be raised upon completion of accept op. </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//AcceptAsync will call the AcceptEventArg_Completed </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//method when it completes, because when we created this SocketAsyncEventArgs </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//object before putting it in the pool, we set the event handler to do it. </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//AcceptAsync returns false if the I/O operation completed synchronously.</font>
    </p>
    <p>
      &#160;&#160;&#160;<font color="#006666">&#160;//The SocketAsyncEventArgs.Completed event on the acceptEventArg </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;//parameter will NOT be raised when AcceptAsync returns false.</font>
    </p>
    <p>
      &#160;&#160;&#160;&#160;if (!willRaiseEvent)
    </p>
    <p>
      &#160;&#160;&#160;&#160;{
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;if (Program.watchProgramFlow == true)&#160;&#160;&#160;//for testing
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;{
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArg.UserToken;
    </p>
    <p>
      
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;Program.testWriter.WriteLine(&quot;StartAccept in if (!willRaiseEvent), accept token id &quot; + theAcceptOpToken.TokenId);
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;}
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;<font color="#006666">//The code in this if (!willRaiseEvent) statement only runs </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;//when the operation was completed synchronously. It is needed because </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;//when Socket.AcceptAsync returns false, </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;//it does NOT raise the SocketAsyncEventArgs.Completed event. </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;//And we need to call ProcessAccept and pass it the SAEA object. </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;//This is only when a new connection is being accepted. </font>
    </p>
    <p>
      <font color="#006666">&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;// Probably only relevant in the case of a socket error.</font>
    </p>
    <p>
      &#160;&#160;&#160;&#160;&#160;&#160;&#160;<b><font color="#0000ff">&#160;ProcessAccept(acceptEventArg);</font></b>
    </p>
    <p>
      &#160;&#160;&#160;&#160;}
    </p>
    <p>
      }
    </p>
  </body>
</html>
</richcontent>
<node CREATED="1421309531553" ID="ID_1087709949" MODIFIED="1421309534737" TEXT="invoke ">
<node CREATED="1421309535905" ID="ID_1609197239" MODIFIED="1421309581277" TEXT="CreateNewSaeaForAccept"/>
<node CREATED="1421309586945" ID="ID_1348109696" MODIFIED="1421309592387" TEXT="ProcessAccept"/>
</node>
</node>
</node>
</node>
</node>
</node>
</node>
</node>
</node>
<node CREATED="1421304410049" ID="ID_1627247667" MODIFIED="1421304424899" TEXT="SocketClientAsyncTester"/>
</node>
</node>
</node>
</map>
