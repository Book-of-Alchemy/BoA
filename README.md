# 조합의 서

![조합의 서 썸네일](https://github.com/user-attachments/assets/23bd7298-fefe-45ab-b2dd-0a3730c1bc47)


---

# 📋 목차

### 1. [프로젝트 개요 및 목표](#1-프로젝트-개요-및-목표)  
### 2. [사용된 기술 스택](#2-사용된-기술-스택)  
### 3. [게임 사이클](#3-게임-사이클)  
### 4. [기술적인 도전 과제](#4-기술적인-도전-과제)  
### 5. [트러블 슈팅](#5-트러블-슈팅)
### 6. [사용자 개선 사항](#6-사용자-개선-사항)  
### 7. [팀원 소개](#7-팀원-소개)  

---

## 🎬 프로젝트 소개 영상

- [게임 트레일러 보기](https://youtu.be/r2rrU3A1IZ4)  
- 던전 내의 재료들로 소모품을 만들어 전투하는 **2D 턴제 로그라이트 게임**

---

## 1️⃣ 프로젝트 개요 및 목표
  **📅프로젝트 개발 기간** : 2025.04.04 ~ 2025.06.02
> 조합의 서는 전략적 로그라이크 던전 탐험 게임으로, 플레이어는 매번 **랜덤하게 생성되는 던전 맵**에서 **몬스터와 아이템을 탐색**하여 퀘스트를 클리어합니다.

- 🔱 퀘스트 목표 달성을 위해 랜덤하게 생성되는 던전 맵 구조를 탐험할 수 있습니다.  
  ![image](https://github.com/user-attachments/assets/7bdc6ea0-3d15-4513-81a2-7ae404f3e445)

- **던전 탐험을 통해 획득한 재료**를 활용하여 **30여 종의 강력한 아이템**을 제작할 수 있습니다.
  
  <img src="https://github.com/user-attachments/assets/a4d2f68e-261a-4b47-b4bb-75dc0778123e"
     width="300px"
     alt="예시 GIF" />
  <img src="https://github.com/user-attachments/assets/c6ce1429-16cf-478a-afd3-bb0c4a72e046"
     width="500px"
     alt="예시 GIF" />

- 각 던전은 **환경 타일의 속성에 따라 다른 전투 반응**을 유도하며, 단순한 공격이 아닌 **전략적 선택**이 요구됩니다.  
  ![기름불](https://github.com/user-attachments/assets/5f66006b-56ad-4512-81d6-d148f4ecd612)
  ![안개2](https://github.com/user-attachments/assets/37967b45-e841-4faf-b65f-34645eb8b1bf)

- 레벨업 시 **60종의 유물 중 랜덤으로 제시된 유물을** 통해 캐릭터의 능력을 강화합니다.  
  ![112](https://github.com/user-attachments/assets/e902f816-e745-4e7c-b684-c558869ed77f)
  ![image](https://github.com/user-attachments/assets/3cd56d20-0419-4f61-9642-4ce3ff0ab88e)

- 성장과 선택한 아이템을 통해 강력한 적에게 도전할 수 있습니다.  
  ![image](https://github.com/user-attachments/assets/4ebd66ee-4289-4cf9-9a0e-ada623abc820)


- 퀘스트 클리어 후에는 **남은 재료를 골드로 환전**할 수 있으며, 골드는 **영구적인 스탯 강화**에 사용되어 반복 도전의 재미를 더합니다.  
  ![image](https://github.com/user-attachments/assets/620b16e1-949d-46ca-8c63-3f2a319a7514)

---

### 프로젝트 시작 시 목표 및 달성 성과

1. **던전 내 제작 → 전투 루프 (기본 전투 사이클)**
   - 소모품 제작 시스템
   - 인벤토리
   - 턴 시스템
   - 전투에 필요한 기본 인터랙션
     - 이동
     - 타겟팅
     - 소모품 사용
   - 몬스터 AI (기본 AI)
   - 피해 처리
   - 턴 처리 시스템

2. **랜덤 맵 생성 + 탐색 시스템**
   - 미니맵
   - 맵 타일 생성
   - Room Cell 기반 랜덤 방 생성
   - 재료/이벤트 배치
   - 몬스터 리젠 시스템

3. **마을 시스템 및 성장 루프 (자원 회수 → 시설 투자 → 성장)**
   - 퀘스트 클리어 시 보상
     - 메인 목표 클리어 시점에 남아 있는 재료를 마을 자원으로 추가 → 골드로 환전
   - 각 시설별 UI 및 기능 (대장간, 탐험기지 등)
     - 연구, 퀘스트 수주 구현 완료

4. **후순위 기능 (추후 개발 예정)**
   - 트랩 시스템
   - 바이옴 시스템
   - ~~생존 제약 시스템~~
   - 바이옴별 지형
   - 서브퀘스트

---

## 2️⃣ 사용된 기술 스택

![image](https://github.com/user-attachments/assets/d34df5d9-b763-4626-a70a-16e16c09a268)

- **프레임워크 & 언어**  
  - C#  
  - .NET 7.0  

- **개발 환경**  
  - Visual Studio 2022  
  - Windows 10  

- **데이터 관리**  
  - Google Spreadsheet  

---

## 3️⃣ 게임 사이클

![image](https://github.com/user-attachments/assets/484431bc-8765-4703-9d10-5cd5d50e9f08)

---

## 4️⃣ 기술적인 도전 과제

- 절차적 맵 생성
    
    ### BSP(Binary Space Partitioning)
    
    ![image](https://github.com/user-attachments/assets/5e1c791c-5eb3-4364-96bd-bf493bb202d0)

    ![image](https://github.com/user-attachments/assets/151406b4-37f5-4427-b3fc-88e7454f37e1)

    - Rect 구조체를 기반으로 하여 재귀적 이분법을 통하여 적절한 크기의 임의의 직사각형 조각을 형성
    
    ### Kruskal MST(쿠르스칼 최소신장트리)
    
    ![image](https://github.com/user-attachments/assets/7f4810b0-d1a9-4e57-8bf5-044229575194)
    
    ![image](https://github.com/user-attachments/assets/1b510961-bde2-4ee2-9940-c0e8b7f74a39)
  
    - Union - Find 메서드를 통하여 각 Rect 간의 연결을 최소한의 거리로 보장
    - 고립되는 방이 생성될 가능성을 배제
    
- StatBlock / StatEntry 구조
    
    
    ![image](https://github.com/user-attachments/assets/87f970f9-19d5-4b55-b1b9-a72a755ac5a2)

    - 도입배경
        - 다양한 요소로 인한 스탯 변화 가능성
        - 본래의 스탯을 유지하며 변형된 스탯을 객체 형태로 저장 및 해제할 필요성 증대
        
    - 개선사항
        - 본래의 스탯 값을 유지하면서 스탯에 변화를 주는 요소들을 저장 및 해제 가능해짐
        - 스탯관리가 일관되도록 유지
- A* 알고리즘
    - 도입배경
        - 2D 타일형 턴제 게임 특성상 길찾기 등 A*알고리즘의 사용처가 다양하게 존재
        - 활용처가 많은 만큼 길찾기 알고리즘을 직접 구현할 필요성을 느낌
    - 개선사항
        - 절차적 맵 생성시 복도 타일 생성에 활용
        - 몬스터 AI 구성시 몬스터의 추적 및 함정 회피
        - 플레이어 마우스 조작시 원하는 타일로 네비게이션하여 이동
- Bresenham 알고리즘
    
    
    ![image](https://github.com/user-attachments/assets/8aa92b34-2b0d-4a87-8b40-be0dfffcb8d4)
    
    - 도입배경
        - 유니티에서 제공되는 RayCast를 이용하여 레스터상에서 직선을 생성할 시 의도되지않은 형태의 레스터 직선을 반환
        - 타일상에 직선을 사용하는 시야, 아이템 사용 등의 알고리즘 필요성 증대
    - 개선사항
        - 고전 알고리즘을 통해 신뢰도 높고 리소스를 최소한으로 사용하여 레스터 직선을 반환
        - 아이템 사용시 방해물 또는 적이 경로상에 있는 경우 해당 방해물 또는 적에게 사용되도록 구현
        - 레스터 상의 시야 구현 시 브레젠험 직선을 활용하여 시야에 막히는 곳이 있는 지 확인하여 레스터 상의 시야를 구현
- Template Method 패턴 - 아이템
    
    ![image](https://github.com/user-attachments/assets/a8d32d4e-a40f-4b44-aea1-53fcd7105aab)

    - 도입배경
        - 하나하나 아이템마다 클래스를 만들기에는 작업량이 많았음
        - 타입별 작동방식은 똑같아서 타입별로 묶어서 사용할 필요성느낌
    - 개선된 사항
        - 타입별 아이템을 생성하기 때문에 여러 아이템들이 나와도 코드 재사용성이 용이함
        - 다른 타입을 추가하더라도 다른 코드를 건들일 필요가 없음
- 윈도우 에디터를 이용한 구글스프레드시트 연동 및 SO생성
    
    ![image](https://github.com/user-attachments/assets/9fec2f2a-3a1c-4674-a2c0-2a1816f9486e)
    
    - 도입배경
        - DB를 SO로 변환하는 방법이 필요했지만 여러가지 방법들을 찾아봤는데 불러옴과 동시에 so로 변환하고 수정해서 사용해야 했음
        - 불러온 것을 json으로 변환하고 그 json을 토대로 so를 생성하고 싶었음
    - 개선된 사항
        - Json버튼과 SO 버튼을 따로 분리하니 매번 구글 스프레드시트에서 Json으로 변환하지 않아도 SO로 변환 가능함
        - 커스텀 에디터로 만들어서 사용하기 쉽게 만듦
        - 구글스프레드시트 경로와 json을 저장할 경로, SO저장할 경로 3가지만 작성 필요
        - UnityWebRequest, Newtonsoft.Json, EditorCoroutines를 사용
        
        ![image](https://github.com/user-attachments/assets/aa5d4bd7-8236-4fb8-a2d6-425c0a78a738)

- 제작 로직
    - 도입배경
        - 제작에 아이템 순서와 상관없이 제작가능하게 하고 싶었음
        - 처음 코드 작성 시 조건이 너무 길어짐
    - 개선된 사항
        - 레시피들을 불러와서 재료들의 ID값을 정렬하고 String으로 Join하여 Key값으로 사용
        - 순서에 대한 조건을 작성할 필요가 없어짐
        - 재료 순서를 뒤죽박죽 넣어도 정상적으로 제작 가능
- 아티팩트 로직
    - 도입배경
        - 아이템과 다르게 하나하나에 구현 필요
    - 개선된 사항
        - Simple Factory패턴으로 아이디 값에 따른 객채생성
        - 아티팩트마다 추상클래스로 상속받아 매서드 구현
        - 아티팩트는 실체화가 되지 않기 때문에 객채 생성하면서 매서드 동작
- 쉐이더 그래프 적용
    
    
    ![image](https://github.com/user-attachments/assets/f5c3def2-69d0-4586-8d8b-e1471078451a)
    
    원본에셋
    
    ![env](https://github.com/user-attachments/assets/265f76d6-5365-4fc7-ac7c-dedba4c921d4)

    쉐이더 그래프 적용 후
    
    ![image](https://github.com/user-attachments/assets/c407c0d3-76ee-43b4-b292-2ceef43878a0)

    - 도입배경
        - 도트 형태가 아닌 에셋 사용시 다른 에셋과의 괴리감 발생
        - 환경 요소들 중 흐르는 물, 용암, 기름, 독 늪, 전기가 흐르는 물 등은 흐르는 효과 필요
        - 별개의 스프라이트 사용시 autotiling을 위한 스프라이트가 요구됨
    - 개선사항
        - 별도의 에셋 없이 도트형 쉐이더를 적용 시킴
        - 하나의 스프라이트로 다양한 환경요소를 일관되게 표현
    
- Strategy 패턴 기반 Behavior시스템
    
    ![image](https://github.com/user-attachments/assets/e874cf7b-c668-4e08-a31c-c4a8de52f203)
    
    - 도입배경
        - 던전조작을 플레이어컨트롤러에 구현했으나 씬마다 다른 조작방식 제공이 필요해짐
    - 개선사항
        - PlayerBaseBehavior 추상 클래스를  기반으로 씬마다 다른 행동패턴 구현
        - 이벤트를 통한 동적인 Behavior 선택
        - DungeonBehavior, TownBehavior 등 씬에 따른 다른 조작 방식 제공
- 이벤트 기반 입력 시스템
    - 도입배경
        - Update, FixedUpdate를 통한 입력시스템이였으나 턴제 게임에선 무의미한 성능낭비라 판단
    - 개선사항
        - InputManager를 통해 입력을 받고 입력이벤트에 각 메서드를 구톡하여 구현
        - 던전조작의 경우 velocity를 통한 움직임에서 Dotween을 사용한 움직임으로 리팩토링하여 더 부드러운 움직임 제공
- Template Method 패턴 기반 씬 관리
    - 도입배경
        - 기존 PlayerController를 동적인 Behavior시스템으로 리팩토링할때 씬 전환 이벤트를 발생시킬 씬 관리 스크립트가 필요해짐.
        - 코드중복을 최소화하며, 변경되지않는 기본 코드 구조, 세부 단계만 수정 용이하며 유지보수가 간편한 구조의 씬매니저를 구상해야했음.
    - 개선사항
        - SceneBase라는 추상 클래스를 통해 씬 생명주기를 관리
        - 씬매니저에선 생성자로 씬을 생성하여 딕셔너리에 넣어 관리
        - 비동기 씬 로딩 구현
        - 씬 전환을 이벤트로 알림
- 옵저버 기반 UI 자동갱신
    - 도입배경
        - 기존 데이터 변경이 생길 때 수동으로 UI 갱신하는 구조에서 오류 및 누락 가능성 존재함.
        - 변동이 생기는 경우마다 일일히 수동으로 갱신코드를 추가/삭제 해야함.
    - 개선사항
        - IObservable 인터페이스로 UI 갱신을 위한 이벤트 인터페이스 생성
        - 갱신이 되는 UI에 상속해서 아이템 갱신 메서드 구독
        - 기존 수동 갱신 코드를 갱신 메서드로 대체
- MVP 패턴 UI
    - 도입배경
        - 인벤토리 UI에서 로직과 UI갱신이 분리되지 않아 역할이 한쪽으로 치중되었음.
        - 변화에 따른 UI갱신의 자동화 및 안정성 확보를 위해서
    - 개선사항
        - UI View - Presenter - Model 분리하여 연동
        - Presenter 클래스에서 이벤트를 구독, 변경이 생기면 View 갱신 메서드 호출
            
            ![image](https://github.com/user-attachments/assets/549aeca6-ebda-4799-b38c-85215174d916)
            
- UI 슬롯 클래스 통합 설계
    - 도입배경
        - 비슷한 기능을 가지는 SlotUI들의 중복 코드 제거
        - 슬롯이 공통된 인터페이스를 상속받아 툴팁 표시, 데이터 자동 갱신을 위함.
    - 개선사항
        - 겹치는 내부 필드, 메서드 통합
        - 각각 고유 기능을 제외한 코드 부모 override 구조로 변경
        - 슬롯 변경시 객체 재생성 없이 처리 가능
- UI 애니메이션
    - 도입배경
        - 구매한 에셋이 프레임 별 애니메이션
        - Animator를 이용한 애니메이션 사용 시 부드러운 전환 필요
    - 개선사항
        - DoTween으로 대체할 수 없는 프레임 별 애니메이션은 Animator로 구현
        - 그 외 대체 가능한 FadeIn/Out, Slide, PopUp등 애니메이션은 DoTween 처리
        - Animator로 애니메이션 동작 시 해당 UI 이미지를 제외한 내부요소가 부자연스럽기때문에
            
            DoTween애니메이션용  UIAnimator클래스로 내부요소가 자연스럽게 등장/퇴장 하도록 수정
            
            ![113](https://github.com/user-attachments/assets/0bdbfb90-af86-4ed9-a70e-cd20a06c66b4)
   

---

## 5️⃣ 트러블 슈팅

- 복잡하고 다양한 변수가 존재하는 경우의 대미지, TileRule 처리
    - 문제되는 현상
        - 대미지 처리, 환경요소의 반응 처리 시 단순한 매개변수만으로는 처리하는데 어려움을 겪었다.
        - 기획상 대미지처리시 한번에 3명 공격시, 피격자의 상태, 공격자의 상태, 특정 스킬의 쿨타임 등등 굉장히 많고 비정형인 형태의 변수들이 산재하였다.
    - 시도
        - 대미지에 관한 매개변수를 구조체로 만들어 일관되게 처리하였다.
        
        ```jsx
        public struct DamageInfo
        {
            public float value;
            public DamageType damageType;
            public CharacterStats source;  // 가해자
            public CharacterStats target;  // 피해자
            public bool isCritical;
            public int statusEffectID;
            public Tag[] tags;
        
            public DamageInfo
                (
                float value, 
                DamageType damageType, 
                CharacterStats source, 
                CharacterStats target, 
                bool isCritical = false,
                Tag[] tags = null, 
                int statusEffectID = -1
                )
            {
                this.value = value;
                this.damageType = damageType;
                this.source = source;
                this.target = target;
                this.isCritical = isCritical;
                this.statusEffectID = statusEffectID;
                this.tags = tags;
            }
        }
        ```
        
    - 해결 방안
        - DTO(Data Transfer Object)를 활용한다면 다양한 매개변수가 필요한 경우 해당하는 매개변수를 한번에 전달할 수 있으며 또한 다른 클래스에서 사용할 때 혼동되는 일 없이 일관되게 작업할 수 있게된다.
    
- 턴 처리 시 지연 문제
    - 문제되는 현상
        - TurnManager에서 턴 처리 시 Enemy의 턴 종료를 기다리기 위해 Coroutine WaitUntil을 사용하여 몬스터가 많아질 경우 몬스터마다 최소 1프레임을 대기하여 조작감을 해치는 문제 발생
    - 시도
        - 현재 시야에 없는 몬스터의 경우 턴 처리 속도 증가
        - TurnManager를 코루틴이 아닌 상태 패턴으로 구성하여 턴처리 진행
            - 이 역시도 update에서 턴처리를 하게되어 대기시간은 여전했음
            - update를 사용하지 않고 콜백을 통해 구성할 경우 구조를 크게 변경하여야 되는 문제 발생
    - 문제 해결
        - Async를 이용하여 최소 대기시간을 ms단위로 변경
        - 구조 변경 최소화
        - 플레이어 캐릭터의 움직임 1f를 제외한 턴처리 속도가 약 0.4초에서 0.05초로 크게 개선
    
    개선 전 턴 처리
    
    ![image](https://github.com/user-attachments/assets/9b2a37fd-2051-43df-a33d-fd138b7fde6d)
    
    Average ≒ 0.4초
    
    개선 후 턴 처리
    
    ![image](https://github.com/user-attachments/assets/2414acd9-74e8-4a57-9ece-127eddf0c71c)
    
    Average ≒ 0.05초
    
- 커스텀에디터 상속으로 인한 코루틴 문제
    - 문제되는 현상
        - Json을 Scriptable Object로 변환하는 과정에서 윈도우 에디터로 커스텀에디터를 만들어서 사용하는 과정에서 윈도우 에디터를 사용시 MonoBehaviour를 상속받지 못해 코루틴을 사용할 수 없는 문제가 발생함
    - 시도
        - EdiorCoroutine 패키지가 있어서 다운받은 후 using Unity.EdiotrCoroutines.Editor; 를 사용
    - 해결 방안
        - EditorCoroutineUtility.StartCoroutineOwnerless(코루틴) 을 사용해서 코루틴이 정상적으로  실행이됨
- JsonUtility는 배열을 단독으로 파싱하지 못하는 문제
    - 문제되는 현상
        - JsonUtility는 배열을 단독으로 파싱하지 못해서 배열 Json을 JsonUtility.FromJson<ItemDataArray>("{\"items\":" + jsonText + "}")  이런식으로 래퍼클래스를 만들고 배열을 감싸는 키로 Json을 감싸야 한다고 해서 이렇게 했는데 문제가 발생함
        
            
    - 시도
        - 시도 1. 확인해 보기 위해 data가 null인지 List가 null인지 확인해봄
        - 결과 : data가 null로 뜸
        - 시도 2. data가 null인 것을 확인했고  ItemData클래스와 json의 필드명이 일치하는지 확인해봄
        - 결과 : 일일이 확인해본 결과 같음
        - 시도 3. 순서는 상관있는지 몰라서 찾아보니 순서는 상관없었음 그래서 enum값이 문제인지 찾아봄
        - 결과 : JsonUtility는 enum필드를 int값으로만 직렬화/역직렬화 함 그래서 구글시트에서 정수로 해야했음
    - 해결 방안
        - newtonsoft.Json이라는 외부 라이브러리가 있어서 설치하여 사용
        - Newtonsoft를 사용시 배열을 래핑하지않아도 바로 파싱가능
        - enum을 문자열로 파싱가능하다
- 인벤토리 아이템 미표시 문제
    - 문제되는 현상
        - 인벤토리 UI를 공용 슬롯을 가지는 제작, 가방, 장비 탭으로 띄우는게 아닌 다른 탭으로 띄웠을때 제작, 가방 ,장비 탭으로 전환시 아이템이 보이지 않는 버그
    - 시도
        - 인벤토리 타입에 따라 아이템 필터링하는 배열 검사
        - 탭 전환이 이루어졌을떄 인벤토리 타입에 맞게 필터링 재시도
    - 해결 방안
        - 애니메이션이 적용된 후 발생한 문제로 탭 전환 시 애니메이션이 인벤토리 내 요소를 잠시 비활성화 후 다시 활성화하여 보여주는 동작을 하고 있음.
        - 이 부분에서 애니메이션이 전환되는 시점에 인벤토리 하위 요소들이 비활성화 일때 필터링을 진행
        - 인벤토리 필터링을 하위요소들이 활성화 된 후 시점으로 변경
- 공격 입력지연 문제
    - 문제되는 현상
        - 플레이어가 연속 공격입력을 하면
        키보드입력의 경우 지연 없이
        입력횟수에 맞게 공격메서드가 호출되었으나,
        마우스클릭의 경우 한번 누르면 입력이 지연됨
    - 시도
        - 디버그로그를 넣어 구독된 메서드 호출이 잘 이뤄지는지 확인
        - 코드에 중단점을 찍어가며 코드 흐름을 확인
    - 해결 방안
        - InputManger의 마우스 클릭 preformed에
        UI위의 클릭일 경우 return하도록 하는 코드가 존재
        DamageText의 Raycast Target을 꺼도 부모 오브젝트가 체크 되어있으면
        UI위 클릭으로 인식됨.
        - 본래 UI위 클릭을 했을때 플레이어가 동작하는것을
        막기 위해 넣은 코드였으나 당시엔 UI가 열림상태에 따라 플레이어 동작메서드 구독을 해제하였기 때문에
        위 코드를 주석처리하였음
        - DamageText가 빠르게 튀어오르도록 수정된 후
        마우스 클릭을 막지않아.
        다시 코드를 사용해도 문제 없게 됨.

## 6️⃣ 사용자 개선 사항

- 조합 리스트 툴팁
    - 사용자 피드백
        - 아이템 제작을 위한 조합식은 있지만 결과 아이템의 설명이 팝업형태로 나왔으면 좋겠다.
    - 개선 사항
        - 아이템 슬롯에 마우스를 올렸을 때와 같이 조합식 아이템에도 설명을 볼 수있는 툴팁 UI 추가
- 조작감 개선
    - 사용자 피드백
        - 던전에서의 조작감 불편함
        - 캐릭터가 한칸한칸 느리게 움직이는 점
    - 개선 사항
        - 턴 처리 방식을 개선하여 턴 처리 속도를 대폭 개선 (몬스터 당 1프레임 ➡ 5ms)
        - UI 클릭 시 이동 방지

---

## 7️⃣ 팀원 소개

| 이름 | 파트 | E-mail | Git-hub |
| --- | --- | --- | --- |
| 양훈모 | 기획 | wing3575@naver.com | https://github.com/limewing |
| 서상원 | 개발 | ssw980@naver.com  | https://github.com/sangweon25 |
| 윤우중 | 개발 | dbsdnwnd1126@gmail.com  | https://github.com/YoonWooJoong |
| 이성재 | 개발 | sungmars1@naver.com |  [https://github.com/sungmars](https://github.com/sungmars?tab=repositories)/ |
| 황희돈 | 개발 | judelover27@gmail.com | https://github.com/judelover27/ |
