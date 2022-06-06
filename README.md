# tempclientmode: 서버로 값 보낼 때 어떤 값인지 알기 위한 이름


↓클라이언트에서 보낼 값

page 0

CLO:  closw 클라이언트 종료 시 mode 값				데이터 형태 -> CLO 


page 1 로그인 페이지

LOG: login 로그인 버튼 눌렀을 때 아이디 비빌번호 함께 전달  		데이터 형태 -> LOG 아이디, 비밀번호
ANA: add new account 회원가입 버튼 눌렀을 때 아이디 비밀번호 함께 전달	데이터 형태 -> ANA 아이디, 비밀번호
MSG: 사용자 이름과 함께 실시간 메세지 보내기				데이터 형태 -> MSG 방 이름, 아이디, 메세지
(방 이름은 두 사용자의 이름을 합쳐서 만듭니다 예) 여동현 , 가나다 = 여동현가나다 방)


page 3 채팅방

CIN: chat in 채팅 참여할 때 참여할 두 사람 아이디 보내기			데이터 형태 -> CIN 본인 아이디, 상대 아이디
MSG: 실시간 메세지						데이터 형태 -> MSG 방 이름, 아이디, 메세지


page 4 지도


LCS: location 클라이언트 gps 위치					데이터 형태 -> LCS X좌표, 좌표

부가기능(꼭 안 써도 됨)

MYL : 내가 속해 있는 방 리스트 알고 싶을 때 아이디와 함께 보내기  똑같이 MYL 형태로 받음	데이터 형태 -> MYL 아이디




↓클라이언트에서 받을 값

page 1 로그인 페이지

ACL: access login 로그인 성공 시 전달					데이터 형태 -> ACL
FAL: fail login 로그인 실패 시 전달					데이터 형태 -> FAL
SAM: same 가입된 아이디가 있을 때					데이터 형태 -> SAM
SCA: success account 회원가입 성공					데이터 형태 -> SCA
NAC: new accounts 사용자 정보가 없으므로 새로운 사용자를 등록합니다 	데이터 형태 -> NAC	
ALF: accountsinfo 계정 이름 보냄  					데이터 형태 -> ALF 이름
MYL: MyRoomList 방 목록 보냄					데이터 형태 -> MYL 3(데이터 갯수), 이름1, 이름 2, 이름3
FLT: friendslist 친구목록 보냄						데이터 형태 -> FLT 3(데이터 갯수), 이름1, 이름 2, 이름3


page 3 채팅

NRM : no room 기존 방이 없어서 새로 만들었단 신호 크게 중요하지 않음 	데이터 형태 -> NRM
TTK : talktalk 채팅방 대화내용 저장된 거 불러오기(대화 내용만)		데이터 형태 -> TTK 3(데이터 갯수), 안녕, 하이요, ㅋㅋㅋ
TNE:  talkname 채팅 보낸 사람 이름 저장된 거 불러오기(사람만)		데이터 형태 -> TNE 3(데이터 갯수), 김똘똘, 민수, 김똘똘


page 4 지도

MXY : mylocation xy 내 좌표 전송					데이터 형태 -> MXY X좌표, Y좌표
OLX : otherlocation x본인 제외 등록된 모든 클라이언트 x 좌표		데이터 형태 -> OLX 3(데이터 갯수) X좌표1, X좌표2, X좌표3
OLY : otherlocation y본인 제외 등록된 모든 클라이언트 y 좌표		데이터 형태 -> OLY 3(데이터 갯수) Y좌표1, Y좌표2, Y좌표3

부가기능(꼭 안 써도 됨)

MYL : 내가 속해 있는 방 리스트 알고 싶을 때 아이디와 함께 보내기 똑같이 MYL 형태로 받음  	데이터 형태 -> MYL 아이디

