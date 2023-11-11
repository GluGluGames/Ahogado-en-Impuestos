using UnityEngine;
using UnityEngine.AI;

namespace BehaviourAPI.UnityToolkit
{
    using Core;
    /// <summary>
    /// The execution context in unity with references to the gameObject and the main components.
    /// </summary>
    public class UnityExecutionContext : ExecutionContext
    {
        /// <summary>
        /// The script that executes the graph with this context.
        /// </summary>
        public Component RunnerComponent { get; private set; }

        /// <summary>
        /// The gameobject of the agent.
        /// </summary>
        public GameObject GameObject { get; private set; }

        /// <summary>
        /// The transform of the agent.
        /// </summary>
        public Transform Transform => GameObject.transform;

        /// <summary>
        /// The NavMeshAgent component.
        /// </summary>
        public NavMeshAgent NavMeshAgent { get; private set; }

        /// <summary>
        /// The animator component.
        /// </summary>
        public Animator Animator { get; private set; }

        /// <summary>
        /// The Rigidbody component.
        /// </summary>
        public Rigidbody Rigidbody { get; private set; }

        /// <summary>
        /// The rigidbody2D component.
        /// </summary>
        public Rigidbody2D Rigidbody2D { get; private set; }

        /// <summary>
        /// The collider component.
        /// </summary>
        public Collider Collider { get; private set; }

        /// <summary>
        /// The collider 2D component.
        /// </summary>
        public Collider2D Collider2D { get; private set; }

        /// <summary>
        /// The component used to interact with smart objects.
        /// </summary>
        public SmartAgent SmartAgent { get; private set; }

        /// <summary>
        /// The characterController component.
        /// </summary>
        public CharacterController CharacterController { get; private set; }

        /// <summary>
        /// The component used by movement actions.
        /// </summary>
        public IMovementComponent Movement { get; private set; }

        /// <summary>
        /// The component used by talk actions.
        /// </summary>
        public ITalkComponent Talk { get; private set; }

        /// <summary>
        /// The component used by talk actions.
        /// </summary>
        public ISoundComponent Sound { get; private set; }

        /// <summary>
        /// The component used by talk actions.
        /// </summary>
        public IRendererComponent Renderer { get; private set; }

        /// <summary>
        /// Create a new unity execution context with a runner script component. Use this constructor
        /// to access methods in the runner component with custom actions or perceptions.
        /// </summary>
        /// <param name="runnerComponent">The runner component.</param>
        public UnityExecutionContext(Component runnerComponent)
        {
            RunnerComponent = runnerComponent;
            GameObject = runnerComponent.gameObject;
            if (GameObject != null)
            {
                NavMeshAgent = GameObject.GetComponent<NavMeshAgent>();
                Rigidbody = GameObject.GetComponent<Rigidbody>();
                Rigidbody2D = GameObject.GetComponent<Rigidbody2D>();
                Collider = GameObject.GetComponent<Collider>();
                Collider2D = GameObject.GetComponent<Collider2D>();
                CharacterController = GameObject.GetComponent<CharacterController>();
                Animator = GameObject.GetComponent<Animator>();
                SmartAgent = GameObject.GetComponent<SmartAgent>();

                Movement = GameObject.GetComponent<IMovementComponent>();
                Talk = GameObject.GetComponent<ITalkComponent>();
                Sound = GameObject.GetComponent<ISoundComponent>();
                Renderer = GameObject.GetComponent<IRendererComponent>();
            }
            else
            {
                Debug.LogError("Context was created with a null component reference");
            }
        }

        /// <summary>
        /// Create a new uniy execution context with a gameobject.
        /// </summary>
        /// <param name="gameObject"></param>
        public UnityExecutionContext(GameObject gameObject)
        {
           
        }
    }
}