using _Project.Scripts.EventBus;
using _Project.Scripts.Managers;
using _Project.Scripts.ScriptableObjects.Classes;
using AbilitySystem;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI {
    public class ClassSelectionUI : MonoBehaviour {
        [SerializeField] SceneControllerSo sceneController;
        [SerializeField] GameScene targetScene;

        [SerializeField] ClassData[] classDataList;
        [SerializeField] Transform classButtonContainer;
        [SerializeField] GameObject classButtonPrefab;
        [SerializeField] Transform classPreviewContainer;
        [SerializeField] Transform classDetailsContainer;
        [SerializeField] TextMeshProUGUI classNameText;
        [SerializeField] TextMeshProUGUI classDescriptionText;

        EventBinding<OnClassSelection> classSelectionBinding;

        ClassData selectedClass;

        void OnEnable() {
            this.classSelectionBinding = new EventBinding<OnClassSelection>(this.Handle_ClassSelection);
            EventBus<OnClassSelection>.Register(this.classSelectionBinding);
        }

        void OnDisable() {
            EventBus<OnClassSelection>.Deregister(this.classSelectionBinding);
        }

        void Handle_ClassSelection(OnClassSelection obj) {
            this.ShowUI();
        }

        void ShowUI() {
            this.selectedClass = null;
            foreach (Transform child in this.classButtonContainer) Destroy(child.gameObject);
            foreach (ClassData classData in this.classDataList) {
                GameObject buttonObject = Instantiate(this.classButtonPrefab, this.classButtonContainer);

                Button classButton = buttonObject.GetComponent<Button>();

                Image buttonImage = buttonObject.GetComponent<Image>();
                buttonImage.sprite = Sprite.Create(
                    classData.ClassIcon.texture,
                    new Rect(0, 0, classData.ClassIcon.texture.width, classData.ClassIcon.texture.height),
                    new Vector2(0.5f, 0.5f)
                );

                classButton.onClick.AddListener(() => this.ShowClassDetails(classData));
            }
        }

        void ShowClassDetails(ClassData classData) {
            this.selectedClass = classData;
            foreach (Transform child in this.classPreviewContainer)
                Destroy(child.gameObject);

            GameObject previewObject = Instantiate(classData.ClassSelectionModel, this.classPreviewContainer);
            previewObject.transform.localPosition = new Vector3(0f, -0.33f, -7f);
            previewObject.transform.localRotation = Quaternion.Euler(0f, 315f, 0f);

            foreach (Transform child in this.classDetailsContainer)
                Destroy(child.gameObject);

            foreach (AbilityData ability in classData.Abilities) {
                GameObject buttonObject = Instantiate(this.classButtonPrefab, this.classDetailsContainer);

                Image buttonImage = buttonObject.GetComponent<Image>();
                buttonImage.sprite = Sprite.Create(
                    ability.Icon.texture,
                    new Rect(0, 0, ability.Icon.texture.width, ability.Icon.texture.height),
                    new Vector2(0.5f, 0.5f)
                );
            }

            this.classNameText.text = classData.ClassName;
            this.classDescriptionText.text = classData.Description;
        }

        public async UniTaskVoid StartGameAsync() {
            ClassSelectionManager.Instance.SelectedClass = this.selectedClass;
            await this.sceneController.ChangeScene(this.targetScene);
        }

        public void StartGame() {
            ClassSelectionManager.Instance.SelectedClass = this.selectedClass;
            _ = this.sceneController.ChangeScene(this.targetScene);
        }
    }
}